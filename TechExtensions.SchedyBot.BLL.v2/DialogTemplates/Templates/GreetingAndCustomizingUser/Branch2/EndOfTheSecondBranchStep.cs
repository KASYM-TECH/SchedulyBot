using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch2
{
    public class EndOfTheSecondBranchStep : IDialogStep
    {
        private string _messageTag = "branch2step0mess";
        
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly UpdateContainer _container;
        
        public EndOfTheSecondBranchStep(IMessageTranslationManger messageTranslationManger,
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
        public int BranchId { get; } = 2;
        public int StepId { get; } = 0;
    }
}
