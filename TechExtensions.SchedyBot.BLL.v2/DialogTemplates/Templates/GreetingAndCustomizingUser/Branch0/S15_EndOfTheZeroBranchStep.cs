using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.TelegramBotLinkModule;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0
{
    public class S15_EndOfTheZeroBranchStep : IDialogStep
    {
        private string _messageTag = "branch0step15mess";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly UpdateContainer _container;
        private readonly TelegramBotLinkManager _telegramBotLinkManager;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;

        public S15_EndOfTheZeroBranchStep(IMessageTranslationManger messageTranslationManger,
            TelegramBotLinkManager telegramBotLinkManager,
            IBotMessageManager botMessageSender, BotReplyKeyboardMarkupHandler markupHandler, UpdateContainer container)
        {
            _telegramBotLinkManager = telegramBotLinkManager;
            _markupHandler = markupHandler;
            _container = container;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }
        
        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            var link = await _telegramBotLinkManager.GenerateLink(_container.Client!.Id);
            await _botMessageSender.Send(string.Format(message, link));
        }

        public async Task<DialogIteration> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            throw new NotImplementedException();
        }
        public bool LastStep { get; set; } = true;
        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 15;
    }
}
