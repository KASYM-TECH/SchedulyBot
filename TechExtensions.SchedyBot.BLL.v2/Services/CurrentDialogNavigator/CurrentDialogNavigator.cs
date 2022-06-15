using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.CurrentDialogNavigator.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.CurrentDialogNavigator;

public class CurrentDialogNavigator : ICurrentDialogNavigator
{
    private readonly IMessageTranslationManger _messageTranslationManger;
    private readonly IBotMessageManager _botMessageManager;
    private readonly TemplateService _templateService;
    private readonly ILogger<CurrentDialogNavigator> _logger;
    private readonly IStepService _stepService;
    
    private readonly UpdateContainer _container;
    
    private readonly ICurrentDialogService _currentDialogService;
    //private readonly ITemplateService _templateService;

    public CurrentDialogNavigator(
        IBotMessageManager botMessageManager,
        IStepService stepService,
        IMessageTranslationManger messageTranslationManger,
        TemplateService templateService,
        ILogger<CurrentDialogNavigator> logger, 
        UpdateContainer container, 
        ICurrentDialogService currentDialogService
        //ITemplateService templateService
        )
    {
        _stepService = stepService;
        _templateService = templateService;
        _botMessageManager = botMessageManager;
        _messageTranslationManger = messageTranslationManger;
        _logger = logger;
        _container = container;
        _currentDialogService = currentDialogService;
        // _templateService = templateService;
    }
    
    public async Task HandleCurrentStep()
    {
        var currentDialog = _container.CurrentDialog!;
        var currentStep = _container.Template!.DialogSteps
            .FirstOrDefault(s => s.BranchId == currentDialog.CurrentBranchId &&
                                 s.StepId == currentDialog.CurrentStepId);
        if (currentStep == null)
            throw new NullReferenceException("Не нашли текущий шаг");
        if (currentDialog.State == CurrentDialogState.Started)
        {
            await currentStep.SendReplyToUser();
            currentDialog.State = CurrentDialogState.InProgress;
            await _currentDialogService.Update(currentDialog);
            return;
        }
        
        var nextDialogIteration = await currentStep.HandleAnswerAndGetNextIteration(_container.Update.Message!.Text!);
        
        //Если есть предопределенный маршрут
        if(currentDialog.StepRoute.Count() != 0)
        {
            var stepComparer = new StepComparer();
            currentDialog.StepRoute.Sort(stepComparer);
            var currentStepIndex = currentDialog.StepRoute.FindIndex(s => s.TemplateId == currentStep.TemplateId && s.BranchId == currentStep.BranchId && s.StepId == currentStep.StepId);
            var stayOnSameStep = nextDialogIteration.TemplateId == currentStep.TemplateId && nextDialogIteration.BranchId == currentStep.BranchId && nextDialogIteration.StepId == currentStep.StepId;
            //Если это был последний степ
            if (currentDialog.StepRoute.Count() - 1 == currentStepIndex && stayOnSameStep is false)
            {
                // Это значит конец текущего диалога
                await _currentDialogService.Update(currentDialog);
                await Cancel(CancelReason.TemplateIsOver);
                return;
            }
            var nextItem = currentDialog.StepRoute[currentStepIndex];
            //Если true, то по правилам текущего шага нужно еще раз по нему пройтись
            if(stayOnSameStep is false)
                nextItem = currentDialog.StepRoute[currentStepIndex + 1];

            _container.Template = _templateService.GetTemplateByEnum((TemplateEnum)nextItem.TemplateId);
            nextDialogIteration = new DialogTemplates.Models.DialogIteration(nextItem.TemplateId, nextItem.BranchId, nextItem.StepId);
            //Повторно отправлять сообщение не нужно если мы остаемся на том же самом степе 
            if (currentDialog.StepRoute.IndexOf(nextItem) == currentStepIndex)
                nextDialogIteration.SendMessage = false;
        }

        var nextStep = _container.Template!.DialogSteps
                        .FirstOrDefault(s => s.BranchId == nextDialogIteration.BranchId &&
                         s.StepId == nextDialogIteration.StepId); 

        if (nextStep is null)
            throw new NullReferenceException("Не удалось вытащить следующий шаг у темплейта");
        
        // Отправляем текст следующего степа клиенту
        if (nextDialogIteration.SendMessage)
            await nextStep.SendReplyToUser();
        // Если следующий степ идентичен текущему, значит какая-то ошибка ввода от юзера, поэтому оставляем состояние текущим
        if (nextStep.Equal(currentStep))
            return;

        //иначе при попытке начать новый темплейт бот думает что старый еще не закончился, он заканчивает его и отправляет Хорошего дня!
        if (nextStep.LastStep)
        {
            // Это значит конец текущего диалога
            await Cancel(CancelReason.TemplateIsOver);
            return;
        }
        // Далее создаем историчность
        var previousIteration = new CurrentDialogIteration
        {
            CurrentDialogId = currentDialog.Id,
            TemplateId = currentDialog.CurrentTemplateId,
            BranchId = currentDialog.CurrentBranchId,
            StepId = currentDialog.CurrentStepId
        };

        // Передаем состояние новой итерации текущему диалогу
        currentDialog.CurrentTemplateId = nextDialogIteration.TemplateId;
        currentDialog.CurrentBranchId = nextDialogIteration.BranchId;
        currentDialog.CurrentStepId = nextDialogIteration.StepId;
        currentDialog.State = CurrentDialogState.InProgress;
        
        currentDialog.CurrentDialogIterations.Add(previousIteration);
        await _currentDialogService.Update(currentDialog);
    }

    public async Task<bool> GoOneStepBackIfNeed()
    {
        var messageText = _container.Update.Message!.Text;
        if (messageText != await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn))
            return false;
        
        var currentDialog = _container.CurrentDialog!;
        var lastIteration = currentDialog.CurrentDialogIterations
            .OrderByDescending(d => d.Id)
            .FirstOrDefault();
        if (lastIteration == null)
        {
            await _botMessageManager.Send(await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtnInvalid));
            // Тут немного нелогично возвращаем true, чтобы на верхнем уровне не пойти дальше по алгоритму
            return true;
        }

        currentDialog.CurrentTemplateId = lastIteration.TemplateId;
        currentDialog.CurrentBranchId = lastIteration.BranchId;
        currentDialog.CurrentStepId = lastIteration.StepId;
        currentDialog.CurrentDialogIterations.Remove(lastIteration);

        await _currentDialogService.Update(currentDialog);
        
        var currentStep = _container.Template!.DialogSteps
            .FirstOrDefault(s => s.BranchId == currentDialog.CurrentBranchId &&
                                 s.StepId == currentDialog.CurrentStepId);
        if (currentStep is null)
            throw new NullReferenceException("Не удалось найти текущий шаг при возврате");
        
        await currentStep.SendReplyToUser();
        
        return true;
    }

    public async Task<bool> CancelIfNeed()
    {
        var messageText = _container.Update.Message!.Text;
        if (messageText != await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.CancelBtn))
            return false;

        await Cancel(CancelReason.Break);
        return true;
    }

    public async Task Cancel(CancelReason reason)
    {
        switch (reason)
        {
            case CancelReason.TemplateIsOver:
                await _botMessageManager.ReturnMainButtonsToClient();
                break;
            //юсер нажал Cancel btn
            case CancelReason.Break:
                await _container.Template!.OnCancel();
                await _botMessageManager.RemoveButtons(await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.CancelBtnReply));
                await _botMessageManager.ReturnMainButtonsToClient();
                break;
            case CancelReason.ChangeDialog:
                break;
        }

        await _stepService.DeleteMany(_container.CurrentDialog!.StepRoute);
        await _currentDialogService.DeleteAllByChatId(_container.ChatId);
    }
}