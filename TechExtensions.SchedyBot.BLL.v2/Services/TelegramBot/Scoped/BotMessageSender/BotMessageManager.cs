using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.Shared.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender
{
    public class BotMessageManager : IBotMessageManager
    {
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly ITelegramBotClient _botClient;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        private readonly UpdateContainer _container;
        
        public BotMessageManager(
            IMessageTranslationManger messageTranslationManger,
            ITelegramBotClient botClient,
            UpdateContainer container, 
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _botClient = botClient;
            _container = container;
            _markupHandler = markupHandler;
            _messageTranslationManger = messageTranslationManger;
        }
        public async Task Send(string message, IReplyMarkup replyKeyboardMarkup = null, long chatId = 0)
        {
            chatId = chatId == 0 ? _container.ChatId : chatId;
            await _botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyKeyboardMarkup);
        }
        public async Task RemoveButtons(string message)
        {
            await _botClient.SendTextMessageAsync(_container.ChatId, message, replyMarkup: new ReplyKeyboardRemove());
        }
        public async Task ReturnMainButtonsToClient()
        {
            var buttons = new List<string>();

            if (_container.Client!.WentThroughFullRegistration is false)
            {
                await SendMenuButtonsToUnregistred();
                return;
            }
            buttons = new List<string> {_messageTranslationManger.GetTextByTag(
                CollectionName.CommonTranslations, MainMenuAction.UpdateSchedule).Result,
               _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations,  MainMenuAction.UpdateProfile).Result,
               _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations,  MainMenuAction.EditService).Result };
            var replyKeyboardMarkup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(buttons);
            var welcome = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.Welcome);
            await Send(welcome + (_container.Client.FirstName.IsNullOrEmpty() ? "" : $", {_container.Client!.FirstName}"), replyKeyboardMarkup);
        }
        private async Task SendMenuButtonsToUnregistred()
        {
            var buttons = new List<string>();
            buttons = new List<string>() { _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, MainMenuAction.GoThroughFullRegistration).Result };
            var message = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GoThroughFullRegistrationFirst);
            var replyKeyboardMarkup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(buttons);
            await Send(message, replyKeyboardMarkup);
            return;
        }

        public async Task EditInlineKeyboard(int CallbackQueryId, string message, InlineKeyboardMarkup inlineKeyboardMarkup)
        {
            await _botClient.EditMessageTextAsync(_container.ChatId, CallbackQueryId, message, replyMarkup: inlineKeyboardMarkup);
        }
        public async Task DeleteInlineKeyboard(int CallbackQueryId)
        {
            await _botClient.DeleteMessageAsync(_container.ChatId, CallbackQueryId);
        }
    }
}
