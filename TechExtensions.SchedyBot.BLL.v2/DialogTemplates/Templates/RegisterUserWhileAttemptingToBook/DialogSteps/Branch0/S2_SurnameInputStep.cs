using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.RegisterUserWhileAttemptingToBook.DialogSteps.Branch0
{
    public class S2_SurnameInputStep : IDialogStep
    {
        private string _messageTag = "branch0step2mess";
        private string _errorMessage = "branch0EnterErr";

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string> { { typeof(S3_NickNameInputStep), "branch0step2btn1" } };

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;

        public S2_SurnameInputStep(IMessageTranslationManger messageTranslationManger, IBotMessageManager botMessageSender)
        {
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }
        
        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await GetMessage(currentDialog, translationCollectionName);
            var btns = await GetButtons(currentDialog, translationCollectionName);
            var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(btns);
            await _botMessageSender.Send(message, markup);
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var buttons = new List<string>();
            foreach (var tag in NextStepTextTag)
            {
                var translatedBtn = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, tag.Value);
                buttons.Add(translatedBtn);
            }
            var buttonTypeAnswers = new ButtonTypeAnswer { Answer = clientAnswer, AnswersCollection = buttons.ToArray() };


            if (!AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect<ButtonTypeAnswer>(AnswerType.Name, buttonTypeAnswers))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            var answerTag = await _messageTranslationManger.GetTagByText(translationCollectionName, clientAnswer);
            var stepProperty = NextStepTextTag.First();

            if (!(stepProperty.Value == answerTag))
                currentDialog.Client.LastName = clientAnswer;

            currentDialog.CurrentStepType = stepProperty.Key.ToString();
            return currentDialog;
        }

        public int TemplateId { get; } = 1;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 2;
        
        public async Task<List<string>> GetButtons(CurrentDialog currentDialog, string translationCollectionName)
        {
            var buttons = await StepService.AddBaseButtons(_messageTranslationManger);
            foreach (var tag in NextStepTextTag)
            {
                var translatedBtn = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, tag.Value);
                buttons.Add(translatedBtn);
            }

            return buttons;
        }

        public async Task<string> GetMessage(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            message = message.Replace("{name}", currentDialog.Client.FirstName);
            return message;
        }
    }
}
