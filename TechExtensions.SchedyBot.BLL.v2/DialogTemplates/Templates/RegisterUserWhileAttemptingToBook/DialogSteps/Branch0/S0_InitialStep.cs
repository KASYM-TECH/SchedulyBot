using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.RegisterUserWhileAttemptingToBook.DialogSteps.Branch0
{
    public class S0_InitialStep : IDialogStep
    {
        private string _messageTag = "branch0step0mess";
        private string _errorMessage = TextTag.BtnErr;

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string>() { { typeof(S1_FirstNameInputStep), "branch0step0btn1" } };


        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        public S0_InitialStep(IMessageTranslationManger messageTranslationManger, IBotMessageManager botMessageSender)
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
            string[] greetingStepButton = { await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, NextStepTextTag[typeof(S1_FirstNameInputStep)]) };

            var buttonTypeAnswers = new ButtonTypeAnswer { Answer = clientAnswer, AnswersCollection = greetingStepButton };

            if (!AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect(AnswerType.Button, buttonTypeAnswers))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }


            var answerTag = await _messageTranslationManger.GetTagByText(translationCollectionName, clientAnswer);
            currentDialog.CurrentStepType = NextStepTextTag.FirstOrDefault(n => n.Value == answerTag).Key.ToString();

            return currentDialog;
        }

        public int TemplateId { get; } = 1;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 0;

        public async Task<string> GetMessage(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            return message;
        }

        public async Task<List<string>> GetButtons(CurrentDialog currentDialog, string translationCollectionName)
        {
            var buttons = new List<string>();
            foreach (var tag in NextStepTextTag)
            {
                var translatedBtn = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, tag.Value);
                buttons.Add(translatedBtn);
            }

            return buttons;
        }
    }
}
