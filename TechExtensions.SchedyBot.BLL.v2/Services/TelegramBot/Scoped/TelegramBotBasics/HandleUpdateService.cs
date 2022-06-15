using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler.DI;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.TelegramBotBasics;

public class HandleUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<HandleUpdateService> _logger;
    private readonly IUpdateHandler _updateHandler;
    private readonly IBotMessageManager _messageManager;
    private readonly UpdateContainer _container;
    private readonly IUpdateMessageService _updateMessageService;
    
    private readonly IClientService _clientService;
    private readonly ICurrentDialogService _currentDialogService;
    private readonly TemplateService _templateService;

    public HandleUpdateService(
        ITelegramBotClient botClient, 
        ILogger<HandleUpdateService> logger, 
        IUpdateHandler updateHandler, 
        IBotMessageManager messageManager, 
        UpdateContainer container, 
        IUpdateMessageService updateMessageService,
        IClientService clientService, 
        ICurrentDialogService currentDialogService, 
        TemplateService templateService)
    {
        _botClient = botClient;
        _logger = logger;
        _updateHandler = updateHandler;
        _messageManager = messageManager;
        _container = container;
        _updateMessageService = updateMessageService;
        _clientService = clientService;
        _currentDialogService = currentDialogService;
        _templateService = templateService;
    }

    public async Task EchoAsync(Update update)
    {
        try
        {
            // Помнить при обработке шагов, что мы так сделали
            if (update.Type == UpdateType.CallbackQuery)
                update.Message = update.CallbackQuery!.Message;
            if (update.Message is null)
                return;

            if (update.CallbackQuery?.Data is not null &&
                update.CallbackQuery.Data == "blank")
                return;

            // Проверим, не дубль ли это текущего апдейта
            var existsUpdate = await _updateMessageService
                .GetByHashCode(update.Id, update.Message.Chat.Id.ToString());
            
            if (existsUpdate != null && existsUpdate.Any())
                return;

            // Такого апдейта нет -- создадим
            var updateMessage = new UpdateMessage { MessageId = update.Id, ChatId = update.Message.Chat.Id.ToString() };
            //Формируем модель с уникальными данными из апдейта и прихраниваем её хэшкод в той же таблице, что и апдейт
            updateMessage.HashCode = updateMessage.GetHashCode();
            await _updateMessageService.Create(updateMessage);

            // Заполним контейнер инфой с текущим состоянием
            await SetDataContainerUp(update);
            
            await _updateHandler.Handle(update);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e,
                $"Произошла ошибка во время обработки следующего запроса: \n{JsonConvert.SerializeObject(update)}");
            await _botClient.SendTextMessageAsync(_container.ChatId,
                $"Произошла следующая ошибка: {e.ToString()}: ChatId: {_container.ChatId}");
        }
    }
    
    private async Task SetDataContainerUp(Update update)
    {
        _container.Update = update;
        _container.ChatId = update.Message!.Chat.Id;
        _container.Language = update.Message.From!.LanguageCode;
        _container.User = update.Message.From!;
        
        try { await SetUser(); }
        catch (Exception e) { throw new ApplicationException("При установке клиента произошла ошибка", e); }
        try { await SetCurrentDialogAndTemplate(); }
        catch (Exception e) { throw new ApplicationException("При установке текущего диалога произошла ошибка", e); }
    }

    private async Task SetUser()
    {
        _container.Client = await _clientService.GetUserByChatId(_container.ChatId);
        if (_container.Client != null)
            return;

        _container.Client = new Client { ChatId = _container.ChatId, Language = _container.Language };
        await _clientService.Create(_container.Client);
        _container.IsNewClient = true;
    }

    private async Task SetCurrentDialogAndTemplate()
    {
        // Если диалога нет, то новый ещё не создаем
        _container.CurrentDialog = await _currentDialogService.GetByChatId(_container.ChatId);
        if (_container.CurrentDialog is null)
            return;
        
        // TODO: Очень не красиво, надо умнее сделать
        _container.Template = _templateService.GetTemplateByEnum((TemplateEnum) _container.CurrentDialog.CurrentTemplateId);
    }
}