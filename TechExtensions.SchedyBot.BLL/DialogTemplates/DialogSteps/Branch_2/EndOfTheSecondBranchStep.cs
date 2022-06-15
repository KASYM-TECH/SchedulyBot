using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.DialogTemplates.DialogSteps.Branch_2
{
    public class EndOfTheSecondBranchStep : IDialogStep
    {
        public Type DialogTemplateType { get; } = typeof(GreetingAndCustomizingUserTemplate);
        private string _messageTag = "branch2step0mess";
        public int DialogStepId { get; } = 0;
        public int DialogBranchId { get; } = 2;

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string>
        {
            //{тип класса в следующем темплейте, null }
        };
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        public EndOfTheSecondBranchStep(IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageSender)
        {
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }
        public Task<CurrentDialog> HandleAnswerFromClient(CurrentDialog currentDialog, string clientAnswer, string translationCollectionName)
        {
            throw new NotImplementedException();
        }

        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await GetMessage(currentDialog, translationCollectionName);
            currentDialog.State = CurrentDialogState.Suspended;
            await _botMessageSender.Send(message);
            await _botMessageSender.SendNeutralMessage();
            currentDialog.CurrentStepType = null;

        }

        public async Task<string> GetMessage(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            return message;
        }
    }
}
