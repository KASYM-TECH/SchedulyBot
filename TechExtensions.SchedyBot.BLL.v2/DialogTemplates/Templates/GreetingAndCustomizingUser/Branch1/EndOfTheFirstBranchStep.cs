using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch1
{
    public class EndOfTheFirstBranchStep : IDialogStep
    {
        private string _messageTag = "branch1step0mess";

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string>
        {
            //{тип класса в следующем темплейте, null }
        };

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly UpdateContainer _container;
        
        public EndOfTheFirstBranchStep(IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageSender, UpdateContainer container)
        {
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _container = container;
        }

        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            await _botMessageSender.Send(message);
        }

        public async Task<DialogIteration> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            throw new NotImplementedException();
        }
        public bool LastStep { get; set; } = true;
        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 1;
        public int StepId { get; } = 0;
    }
}
