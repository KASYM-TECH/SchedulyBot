using TechExtensions.SchedyBot.DLL.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers.Abstractions;
using TechExtensions.SchedyBot.BLL.Models;
using TechExtensions.SchedyBot.BLL.Services.Abstractions;
using TechExtensions.SchedyBot.BLL.TelegraamUpdateHandler.Abstractions;

namespace TechExtensions.SchedyBot.BLL.TelegraamUpdateHandler
{
    public class TelegramUpdateHandler : ITelegramUpdateHandler
    {
        private readonly IEnumerable<IUpdateManager> _managers;
        private readonly IUpdateMessageService _updateMessageService;
        private readonly IChatIDContainer _chatIDContainer;
        private readonly ILanguageInfoContainer _languageInfoContainer;
        private readonly IUpdateContainer _updateContainer;
        private readonly ICurrentDialogNavigator _dialogNavigator;
        private readonly IClientAndDialogExistenceService _existenceService;
        private readonly ILogger<TelegramUpdateHandler> _logger;
        
        public TelegramUpdateHandler(
            IUpdateMessageService updateMessageService,
            IUpdateContainer updateContainer,
            ILanguageInfoContainer languageInfoContainer,
            ILogger<TelegramUpdateHandler> logger,
            IChatIDContainer chatIDContainer,
            IEnumerable<IUpdateManager> managers, 
            ICurrentDialogNavigator dialogNavigator, IClientAndDialogExistenceService existenceService)
        {
            _updateContainer = updateContainer;
            _languageInfoContainer = languageInfoContainer;
            _chatIDContainer = chatIDContainer;
            _updateMessageService = updateMessageService;
            _logger = logger;
            _managers = managers;
            _dialogNavigator = dialogNavigator;
            _existenceService = existenceService;
        }

        public async Task Handle(Update update)
        {
            // TODO: думаю многовато контейнеров, если они по сути принимают одно и то же
            _updateContainer.Update = update;
            _chatIDContainer.SetChatId(update);
            _languageInfoContainer.SetLanguageCode(update);
            if (update.Message == null)
                AddMessageToUpdate(update);
            
            var updateExists = await DoesUpdateExist(update.Id, _chatIDContainer.ChatId.ToString());
            if (updateExists)
                return;
            
            var client = await _existenceService.ReturnExistClientOrJustCreated();
            var currentDialog = await _existenceService.ReturnExistDialogOrJustCreated(client);
            // _dialogNavigator.SetCurrentDialog(currentDialog);

            var modelForManage = new UpdateModelForManage
            {
                Update = update,
                Client = client
            };
            
            foreach (var manager in _managers)
            {
                if (await manager.DoesItForMe(modelForManage))
                    await manager.Manage(modelForManage);
            }

            await DeleteUpdateMessage(update.Id, _chatIDContainer.ChatId.ToString());
        }
        private void AddMessageToUpdate(Update update)
        {
            var chatId = update.CallbackQuery.Message.Chat.Id;
            var newChat = new Chat { Id = _chatIDContainer.ChatId };
            var newMessage = new Message { Chat = newChat, Text = update.CallbackQuery.Data };
            update.Message = newMessage;
           
        }
        private async Task<bool> DoesUpdateExist(int updateId, string chatId)
        {
            // TODO: Так все таки updateId или messageId...? Судя по предыдущему коду updateId
            // TODO: Так же здесь не фильтруем по chatId
            // var foundUpdateMessage =  _updateMessageService.Get(u => u.MessageId == updateId);
            // if (foundUpdateMessage != null)
            //     return true;

            await _updateMessageService.Create(new UpdateMessage { UpdateId = updateId, ChatId = chatId });

            return false;
        }

        private async Task DeleteUpdateMessage(int messageId, string chatId)
        {
            // var foundUpdateMessage =  await _updateMessageService.Get(u => u.ChatId == chatId);
            // // TODO: здесь как мне кажется выполняется двойная работа
            // await _updateMessageService.Delete(foundUpdateMessage);
            await _updateMessageService.DeleteAllPreceding(messageId, chatId);
            _logger.LogInformation("дубликат Update удален");

        }
    }
}
