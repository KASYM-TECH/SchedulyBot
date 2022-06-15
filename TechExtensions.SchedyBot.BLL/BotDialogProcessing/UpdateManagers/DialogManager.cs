using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.Extentions;
using TechExtensions.SchedyBot.BLL.Models.Constants;
using TechExtensions.SchedyBot.DLL.Entities;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers.Abstractions;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.DialogTemplates.DialogServices.DI;
using TechExtensions.SchedyBot.BLL.BotMessageSender;
using TechExtensions.SchedyBot.BLL.Models;
using TechExtensions.SchedyBot.BLL.Services.Abstractions;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.UpdateManagers
{
    public class DialogManager : IUpdateManager
    {
        private readonly ICurrentDialogService _currentDialogService;
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageManager;
        private readonly ITemplateService _templateService;
        private readonly ICurrentDialogNavigator _dialogNavigator;
        private readonly ILogger<DialogManager> _logger;
        private readonly IUpdateContainer _updateContainer;

        public DialogManager(ICurrentDialogService currentDialogService,
            IUpdateContainer updateContainer,
            IMessageTranslationManger messageTranslationManger,
            ILogger<DialogManager> logger,
            IBotMessageManager botMessageManager,
            ITemplateService templateService,
            ICurrentDialogNavigator dialogNavigator)
        {
            _updateContainer = updateContainer;
            _logger = logger;
            _messageTranslationManger = messageTranslationManger;
            _currentDialogService = currentDialogService;
            _botMessageManager = botMessageManager;
            _templateService = templateService;
            _dialogNavigator = dialogNavigator;
        }

        public async Task<bool> Manage(UpdateModelForManage model)
        
        {
            ///////////// Первая часть
            var message = model.Update.Message!;

            // var wasTransfered = await TransferAnswerRightToStepIfNeed(model.Update, _dialogNavigator.GetCurrentDialog());
            // if (wasTransfered) 
                return await _dialogNavigator.TryToSaveCurrentDialog();

            //телеграм клиент не начал никакой CurrentDialog до этого, и сейчас его сообщение не является командой темплейта
            // if (_dialogNavigator.GetCurrentDialog().State == CurrentDialogState.Suspended)
            //     return true;
            //////////////

            /////////////// Вторая часть
            //начиная отсюда currentDialog.State == CurrentDialogState.BeingUsedByTemplate верно 100%
            //найденный темплейт
            // var foundTemplate = _templateService.GetDialogTemplateByStep(_dialogNavigator.GetCurrentDialog().CurrentStepType);

            //Отмени темплейт если нужно
            var templateIsCanceled = await CancelTemplateIfNeed(message.Text);
            if (templateIsCanceled)
                return await _dialogNavigator.TryToSaveCurrentDialog();

            //верни пользователя на один шаг назад если нужно
            // var isGoneOneStepBack = await _dialogNavigator.GoOneStepBackIfNeed(foundTemplate, message.Text);

            //пользователь вернулся на один шаг назад
            // if (isGoneOneStepBack)
            // {
            //     var previousStep = foundTemplate.GetStepByClassName(_dialogNavigator.GetCurrentDialog().CurrentStepType);
            //     await previousStep.SendReplyToUser(_dialogNavigator.GetCurrentDialog(), foundTemplate.TranslationCollectionName);
            //     return await _dialogNavigator.TryToSaveCurrentDialog();
            // }

            // var oldDialogCurrentStepType = _dialogNavigator.GetCurrentDialog().CurrentStepType;

            //обрабатываем ответ клиента
            // var handledCurrentDialog = await TransferClientAnswerToStepAnswerHandler(message.Text, foundTemplate, _dialogNavigator.GetCurrentDialog());

            //Диалог закончился 
            // if (handledCurrentDialog.State == CurrentDialogState.Suspended)
            // {
            //     await _botMessageManager.SendNeutralMessage();
            //     return true;
            // }

            //если обработчик степа поменял текущий темплейт
            // var newTemplate = await _dialogNavigator.ChangeTemplateIfNeed();
            // newTemplate = newTemplate == null ? foundTemplate : newTemplate;
            
            //просто отправь следующее сообщение 
            // await _dialogNavigator.HandleNextStep(newTemplate);
            /////////////////////

            await _dialogNavigator.TryToSaveCurrentDialog();
            return true;
        }
        
        private async Task<bool> TransferAnswerRightToStepIfNeed(Update update, CurrentDialog currentDialog)
        {
            if (update.CallbackQuery == null)
                return false;
            var foundTemplate = _templateService.GetDialogTemplateByStep(currentDialog.CurrentStepType);
            await TransferClientAnswerToStepAnswerHandler(update.Message.Text, foundTemplate, currentDialog);
            
            //CurrentInlineKeyboard завершил свою работу 
            // if (currentDialog.CurrentInlineKeyboard == null)
                // await _dialogNavigator.FindAndSendIfNeedNextMessage(foundTemplate);
            
            return true;
        }

        private async Task<CurrentDialog> TransferClientAnswerToStepAnswerHandler(string answer, IDialogTemplate dialogTemplate, 
            CurrentDialog currentDialog)
        {
            var currentStep =  dialogTemplate.GetStepByClassName(currentDialog.CurrentStepType);
            await currentStep.HandleAnswerFromClient(currentDialog, 
                answer, dialogTemplate.TranslationCollectionName);
            return currentDialog;
        }

        private async Task<bool> CancelTemplateIfNeed(string message)
        {
            // var needToCancel = await _dialogNavigator.CancelTemplateIfNeed(message);
            // if (!needToCancel)
                return false;
            
            await _botMessageManager.SendNeutralMessage();

            if (_updateContainer.Update.CallbackQuery != null)
                await _botMessageManager.DeleteInlineKeyboard(_updateContainer.Update.CallbackQuery.Message.MessageId);

            return true;
        }

        /// <summary>
        /// Если диалог существует или ответ пользователя это команда для старта диалога, то тогда возвращаем true
        /// TODO: Некорректное условие, так как у нас всегда будет существовать диалог как минимум, так как мы его создаем если его нет
        /// TODO: Продумать какие условия должны быть у этого менеджера, которые будут отличать его от других
        /// TODO: предположил, что это наличие месседжа
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<bool> DoesItForMe(UpdateModelForManage model)
        {
            if (model.Update.Message != null)
                return true;
            
            return false;
        }
    }
}
