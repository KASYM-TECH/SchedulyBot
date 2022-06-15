using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.Enums;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.CurrentDialogNavigator.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.TelegramBotLinkModule;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler.DI;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.Shared.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler;

public sealed class UpdateHandler : IUpdateHandler
{
    private readonly IUpdateMessageService _updateMessageService;
    private UpdateContainer _container;
    private readonly ILogger<UpdateHandler> _logger;
    
    private readonly IClientService _clientService;
    private readonly ICurrentDialogService _currentDialogService;
    private readonly TemplateService _templateService;
    private readonly IBookingService _bookingService;

    private readonly ICurrentDialogNavigator _dialogNavigator;
    private readonly IBotMessageManager _messageManager;
    private readonly BotReplyKeyboardMarkupHandler _markupHandler;
    private readonly TelegramBotLinkManager _telegramBotLinkManager;
    private readonly IMessageTranslationManger _messageTranslationManger;
    private readonly IServiceAndSpecService _serviceAndSpecService;
    private readonly MainMenuManager _mainMenuManager;


    public UpdateHandler(
        ILogger<UpdateHandler> logger,  
        UpdateContainer container, 
        IUpdateMessageService updateMessageService, 
        IClientService clientService, 
        IMessageTranslationManger messageTranslationManger,
        ICurrentDialogService currentDialogService,
        IBookingService bookingService,
        TemplateService templateService, 
        ICurrentDialogNavigator dialogNavigator,
        MainMenuManager mainMenuManager,
        IServiceAndSpecService serviceAndSpecService,
        TelegramBotLinkManager telegramBotLinkManager,
        IBotMessageManager messageManager, BotReplyKeyboardMarkupHandler markupHandler)
    {
        _logger = logger;
        _container = container;
        _updateMessageService = updateMessageService;
        _messageTranslationManger = messageTranslationManger;
        _clientService = clientService;
        _mainMenuManager = mainMenuManager;
        _currentDialogService = currentDialogService;
        _templateService = templateService;
        _serviceAndSpecService = serviceAndSpecService;
        _dialogNavigator = dialogNavigator;
        _messageManager = messageManager;
        _markupHandler = markupHandler;
        _telegramBotLinkManager = telegramBotLinkManager;
        _bookingService = bookingService;
    }

    public async Task Handle(Update update)
    {
        try
        {
            // Если есть entities, значит это команда
            if (update.Message?.Entities is not null)
            {
                if (update.Message.Text!.StartsWith(TelegramCommand.GenerateLink))
                {
                    await SendLinkToAccount();
                    return;
                }
                if (update.Message.Text!.StartsWith(TelegramCommand.LastBookings))
                {
                    await SendLastSellers();
                    return;
                }

                //если юсер бронирует по ссылке  
                await HandleIfLink(update.Message.Text);

                // Обработчик команд начинает работу
                if (update.Message.Text!.StartsWith(TelegramCommand.Start))
                    await HandleStartCommand(update.Message.Text);

                if (update.Message.Text!.StartsWith(TelegramCommand.OutgoingBookings)
                    || update.Message.Text!.StartsWith(TelegramCommand.IncomingBookings))
                    await HandleBookingsCommandInMessage(update.Message.Text);

                // TODO: Некрасиво, но пока так
                if (_container.CurrentDialog is null && update.CallbackQuery?.Data is null)
                    return;
            }

            // Если есть inline command
            if (update.CallbackQuery?.Data is not null &&
                update.CallbackQuery.Data.Contains('/'))
            {
                // Обработчик команд начинает работу
                if (update.CallbackQuery.Data.StartsWith(TelegramCommand.Book))
                    await HandleBookingStart(update.CallbackQuery.Data);

                if (update.CallbackQuery.Data.StartsWith(TelegramCommand.Start))
                    await HandleStartCommand(update.CallbackQuery.Data);

                if (update.CallbackQuery!.Data.StartsWith(TelegramCommand.OutgoingBookings) || update.CallbackQuery!.Data.StartsWith(TelegramCommand.IncomingBookings))
                    await HandleBookingsCommandInQuery(update.CallbackQuery!.Data);

                // TODO: Некрасиво, но пока так
                if (_container.CurrentDialog is null)
                    return;
            }

            // Команда не распознана, значит это ответ, обрабатываем (ответ может быть как на действующий диалог, так и пунктом меню
            // Если текущий диалог не запущен, значит это пункт меню
           await _mainMenuManager.Manage(update.Message!.Text!);

            if (_container.CurrentDialog == null)
                return;
            // Теперь диалог точно существует или продолжается предыдущий, поэтому погнали выполнять его
            // Отменим, если требуется
            if (await _dialogNavigator.CancelIfNeed())
                return;

            // Вернемся на шаг назад, если требуется
            if (await _dialogNavigator.GoOneStepBackIfNeed())
                return;

            // Назад не возвращаемся, значит идем дальше по алгоритму диалога
            await _dialogNavigator.HandleCurrentStep();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            // Напоследок надо избавиться от UpdateId
            await _updateMessageService.Deactivate(_container.Update.Id);
        }
    }
    private async Task HandleIfLink(string text)
    {
        var keyExists = _telegramBotLinkManager.KeyExists(text);
        if (keyExists)
            await HandleStartCommand(text);

        if (text.Contains(TelegramBotLinkManager.StaticLinkCode) && keyExists is false)
            await _messageManager.Send(_messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.OutdatedLink).Result);
    }
    private async Task HandleBookingStart(string callBackData)
    {
        var id = callBackData.Split(" ")[1];
        _container.Update.Message!.Text = _telegramBotLinkManager.FormatIdToFitString(id);
        await LuanchBookingTemplate();
    }

    private async Task SendLinkToAccount()
    {
        if (_container.Client!.Schedules.Any() && _serviceAndSpecService.GetByClientId(_container.Client.Id).Result != null)
        {
            var messageLink = _telegramBotLinkManager.GenerateLink(_container.Client.Id).Result + "\n" + _container.Client.GetFullName();
            await _messageManager.Send(messageLink);
            return;
        }
        var message = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.CreateScheduleFirst);
        await _messageManager.Send(message);
    }
    private async Task HandleBookingsCommandInQuery(string queryText)
    {
        if (_container.Client!.WentThroughFullRegistration is false)
        {
            var message = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GoThroughFullRegistrationFirst);
            await _messageManager.Send(message);
            return;
        }
        var template = _templateService.GetTemplateByEnum(TemplateEnum.BookingManagmentTemplate);
        _container.Template = template;

        var currentPosition = template!.GetBranchAndStepByState(TemplateStateEnum.IncomingBookingManage);
        if (queryText!.StartsWith(TelegramCommand.OutgoingBookings))
            currentPosition = template!.GetBranchAndStepByState(TemplateStateEnum.OutgoingBookingManage);
        if(_container.CurrentDialog != null)
            await _dialogNavigator.Cancel(CancelReason.ChangeDialog);

        var dialogState = CurrentDialogState.InProgress;
        if(queryText.Contains(BookingAction.info.ToString()))
            dialogState = CurrentDialogState.Started;

        // Теперь создадим текущий диалог
        var dialog = new CurrentDialog
        {
            ChatId = _container.ChatId,
            CurrentTemplateId = (int)template.TemplateId,
            CurrentBranchId = currentPosition!.Value.branchId,
            CurrentStepId = currentPosition.Value.stepId,
            State = dialogState
        };

        await _currentDialogService.CreateCurrentDialog(dialog);
        _container.CurrentDialog = dialog;
    }
    private async Task SendLastSellers()
    {
        var usedSellerIds = _container.Client!.UsedSellerIds;
        if(usedSellerIds == null || usedSellerIds.Count() == 0)
        {
            var noSellerIdsYet = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.NoSellerIdsYet);
            await _messageManager.Send(noSellerIdsYet);
            return;
        }
        var buttons = new List<List<InlineKeyboardButton>>();
        foreach (var sellerId in usedSellerIds)
        {
            var service = await _serviceAndSpecService.GetByClientIdUntracked((int)sellerId);
            var sellerBtn = new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton($"[{service!.Service.Name}] {service.User.GetFullName()}")
                {
                    CallbackData = TelegramCommand.Book + sellerId
                }
            };
            buttons.Add(sellerBtn);
        }
        var message = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.SelectSeller);
        var markup = new InlineKeyboardMarkup(buttons);
        await _messageManager.Send(message, markup);
    }
    private async Task HandleBookingsCommandInMessage(string messageText)
    {
        if(_container.Client!.WentThroughFullRegistration is false)
        {
            var message = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GoThroughFullRegistrationFirst);
            await _messageManager.Send(message);
            return;
        }
        var template = _templateService.GetTemplateByEnum(TemplateEnum.BookingManagmentTemplate);
        _container.Template = template;

        var currentPosition = template!.GetBranchAndStepByState(TemplateStateEnum.IncomingBookingsStart);
        if(messageText!.StartsWith(TelegramCommand.OutgoingBookings))
            currentPosition = template!.GetBranchAndStepByState(TemplateStateEnum.OutgoingBookingsStart);
        var dialogState = CurrentDialogState.Started;
        // Теперь создадим текущий диалог
        var dialog = new CurrentDialog
        {
            ChatId = _container.ChatId,
            CurrentTemplateId = (int)template.TemplateId,
            CurrentBranchId = currentPosition!.Value.branchId,
            CurrentStepId = currentPosition.Value.stepId,
            State = dialogState
        };

        await _currentDialogService.CreateCurrentDialog(dialog);
        _container.CurrentDialog = dialog;
    }


    private async Task HandleStartCommand(string messageText)
    {
        var entities = messageText.Split(' ');
        if (entities.Length == 1)
        {
            await _messageManager.ReturnMainButtonsToClient();
            return;
        }

        if(_telegramBotLinkManager.KeyExists(messageText) is false)
            return;
        var id = Convert.ToInt32(_telegramBotLinkManager.GetIdByLink(messageText));
        _container.Client!.UsedSellerIds.Add(id);
        _container.Client!.UsedSellerIds = _container.Client!.UsedSellerIds.Distinct().TakeLast(10).ToList();
        _container.Update.Message!.Text = _telegramBotLinkManager.FormatIdToFitString(id.ToString());

        await LuanchBookingTemplate();
    }
    private async Task LuanchBookingTemplate()
    {
        // Находим нужный темплейт
        var template = _templateService.GetTemplateByEnum(TemplateEnum.BookSellerTimeTemplate);
        _container.Template = template;

        var currentPosition = template!.GetBranchAndStepByState(TemplateStateEnum.Start);
        if (_container.Client!.WentThroughFullRegistration)
            currentPosition = template!.GetBranchAndStepByState(TemplateStateEnum.ClientExists);

        if (currentPosition is null)
            return;

        // Теперь создадим текущий диалог
        var dialog = new CurrentDialog
        {
            ChatId = _container.ChatId,
            CurrentTemplateId = (int)template.TemplateId,
            CurrentBranchId = currentPosition.Value.branchId,
            CurrentStepId = currentPosition.Value.stepId,
            State = CurrentDialogState.Started
        };

        await _currentDialogService.CreateCurrentDialog(dialog);
        _container.CurrentDialog = dialog;
    }
}