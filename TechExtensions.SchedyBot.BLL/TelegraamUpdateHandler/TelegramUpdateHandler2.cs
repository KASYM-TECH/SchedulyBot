using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.BotMessageSender;
using TechExtensions.SchedyBot.BLL.BotReplyKeyboardMarkupManager;
using TechExtensions.SchedyBot.BLL.Services;
using TechExtensions.SchedyBot.BLL.Services.Abstractions;
using TechExtensions.SchedyBot.BLL.TelegraamUpdateHandler.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TechExtensions.SchedyBot.BLL.TelegraamUpdateHandler;

public sealed class TelegramUpdateHandler2 : ITelegramUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateMessageService _updateMessageService;
    private UpdateDataContainer _dataContainer;
    private readonly ILogger<TelegramUpdateHandler2> _logger;
    
    private readonly IClientService _clientService;
    private readonly ICurrentDialogService _currentDialogService;
    private readonly ITemplateService _templateService;
    
    private readonly ICurrentDialogNavigator _dialogNavigator;
    private readonly IBotMessageManager _messageManager;

    public TelegramUpdateHandler2(
        ITelegramBotClient botClient, 
        ILogger<TelegramUpdateHandler2> logger, 
        MessageAnswerHandler messageAnswerHandler, 
        UpdateDataContainer dataContainer, 
        IUpdateMessageService updateMessageService, 
        IClientService clientService, 
        ICurrentDialogService currentDialogService, 
        ITemplateService templateService, 
        ICurrentDialogNavigator dialogNavigator,
        IBotMessageManager messageManager)
    {
        _botClient = botClient;
        _logger = logger;
        _messageAnswerHandler = messageAnswerHandler;
        _dataContainer = dataContainer;
        _updateMessageService = updateMessageService;
        _clientService = clientService;
        _currentDialogService = currentDialogService;
        _templateService = templateService;
        _dialogNavigator = dialogNavigator;
        _messageManager = messageManager;
    }

    public async Task Handle(Update update)
    {
        try
        {
            if (update.Type == UpdateType.CallbackQuery)
                update.Message = update.CallbackQuery!.Message;

            // Заполним контейнер инфой с текущим состоянием
            await SetDataContainerUp(update);
            
            // Проверим, не дубль ли это текущего апдейта
            var existsUpdate = await _updateMessageService
                .GetByChatIdAndMessageId(_dataContainer.Update.Id, _dataContainer.ChatId.ToString());
            if (existsUpdate != null && existsUpdate.Any())
                return;
            
            // Такого апдейта нет -- создадим
            await _updateMessageService.Create(new UpdateMessage
                { UpdateId = update.Id, ChatId = _dataContainer.ChatId.ToString() });

            // Если есть entities, значит это команда
            if (update.Message!.Entities is not null)
            {
                // Обработчик команд начинает работу
                if (update.Message!.Text == TelegramCommand.Start)
                    await HandleStartCommand();
                return;
            }

            // Команда не распознана, значит это ответ, обрабатываем (ответ может быть как на действующий диалог, так и пунктом меню
            // Если текущий диалог не запущен, значит это пункт меню
            if (_dataContainer.CurrentDialog is null)
                switch (update.Message!.Text)
                {
                    case MainMenuAction.UpdateSchedule:
                        await RunUpdateSchedule();
                        break;
                    case MainMenuAction.UpdateProfile:
                        break;
                    case MainMenuAction.SomethingElse:
                        break;
                    default: break;
                }
            
            if (await _dialogNavigator.GoOneStepBackIfNeed())
                return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            // Напоследок надо избавиться от UpdateId
            await _updateMessageService.Deactivate(_dataContainer.Update.Id);
        }
    }

    /// <summary>
    /// Пока что здесь будет только создание нового расписания
    /// TODO: Редактирование существующего
    /// </summary>
    private async Task RunUpdateSchedule()
    {
        // Пункт меню запускает темплейт создания расписания, поэтому создадим диалог с ним
        var template = 
        await _currentDialogService.CreateCurrentDialog(new CurrentDialog());
    }

    private async Task HandleStartCommand()
    {
        if (!_dataContainer.IsNewClient)
            await ReturnMainButtonsToClient();
    }

    private async Task ReturnMainButtonsToClient()
    {
        var buttons = new List<string> { "Редактировать расписание", "Редактировать профиль", "Что-то ещё" };
        var replyKeyboardMarkup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(buttons);
        await _messageManager.Send($"Добро пожаловать, {_dataContainer.Client!.FirstName}", replyKeyboardMarkup, _dataContainer.ChatId);
    }

    private async Task SetDataContainerUp(Update update)
    {
        _dataContainer.Update = update;
        _dataContainer.ChatId = update.Message!.Chat.Id;
        _dataContainer.Language = update.Message.From!.LanguageCode;
        
        try { await SetClient(); }
        catch (Exception e) { throw new ApplicationException("При установке клиента произошла ошибка", e); }
        try { await SetCurrentDialog(); }
        catch (Exception e) { throw new ApplicationException("При установке текущего диалога произошла ошибка", e); }
        try { await SetDialogTemplate(); }
        catch (Exception e) { throw new ApplicationException("При установке темплэйта произошла ошибка", e); }
        try { await SetDialogStep(); }
        catch (Exception e) { throw new ApplicationException("При установке степа произошла ошибка", e); }
    }

    private async Task SetClient()
    {
        _dataContainer.Client = await _clientService.GetUserByChatId(_dataContainer.ChatId);
        if (_dataContainer.Client != null)
            return;

        _dataContainer.Client = new UserDao { ChatId = _dataContainer.ChatId, Language = _dataContainer.Language };
        await _clientService.Create(_dataContainer.Client);
        _dataContainer.IsNewClient = true;
    }

    private async Task SetCurrentDialog()
    {
        // Если диалога нет, то новый ещё не создаем
        _dataContainer.CurrentDialog = await _currentDialogService.GetCurrentDialogByChatId(_dataContainer.ChatId);
    }

    private async Task SetDialogTemplate()
    {
        if (_dataContainer.CurrentDialog?.CurrentStepType == null)
            return;
        
        _dataContainer.Template = _templateService.GetDialogTemplateByStep(_dataContainer.CurrentDialog!.CurrentStepType);
    }

    private async Task SetDialogStep()
    {
        if (_dataContainer.Template == null)
            return;
        _dataContainer.Step = _dataContainer.Template.GetStepByClassName(_dataContainer.CurrentDialog!.CurrentStepType);
    }
}