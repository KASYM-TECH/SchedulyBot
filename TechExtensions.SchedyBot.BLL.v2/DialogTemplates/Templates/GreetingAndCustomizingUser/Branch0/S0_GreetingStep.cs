using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0
{
    public class S0_GreetingStep : IDialogStep
    {
        private string _messageTag = "branch0step0mess";
        private string _buttonTag = "branch0step0btn1";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageManager;
        private readonly UpdateContainer _container;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        
        public S0_GreetingStep(
            IMessageTranslationManger messageTranslationManger, 
            IBotMessageManager botMessageManager,
            UpdateContainer container, BotReplyKeyboardMarkupHandler markupHandler)
        {
            _messageTranslationManger = messageTranslationManger;
            _botMessageManager = botMessageManager;
            _container = container;
            _markupHandler = markupHandler;
        }
        
        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            var buttonText = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _buttonTag);
            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(new List<string>{buttonText}, false, true);
            await _botMessageManager.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            return this.NextStep();
        }

        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 0;
    }
}
