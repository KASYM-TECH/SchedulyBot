using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch1;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0
{
    public class S4_WannaCreateScheduleStep : IDialogStep
    {
        private string _messageTag = "branch0step4mess";
        private string _errorMessage = TextTag.BtnErr;

        private readonly Dictionary<string, DialogIteration> _currentStepTextTags;

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly UpdateContainer _container;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        
        public S4_WannaCreateScheduleStep(
            IMessageTranslationManger messageTranslationManger, 
            IBotMessageManager botMessageSender, 
            UpdateContainer container, 
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _container = container;
            _markupHandler = markupHandler;
            
            _currentStepTextTags = new Dictionary<string, DialogIteration>
            {
                {
                    TextTag.YesBtn, new DialogIteration
                    {
                        TemplateId = TemplateId,
                        BranchId = BranchId,
                        // Пропускаем выбор сферы услуг
                        StepId = StepId + 2
                    }
                },
                {
                    TextTag.NoBtn, new DialogIteration
                    {
                        TemplateId = TemplateId,
                        BranchId = 1,
                        StepId = 0
                    }
                }
            };
        }
        
        public async Task SendReplyToUser()
        {
            var message =  await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            var btns = await GetButtons();
            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(btns, true, true);
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            _container.Client!.WentThroughFullRegistration = true;
            var buttons = new List<string>();
            foreach (var textTag in _currentStepTextTags)
            {
                buttons.Add(await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, textTag.Key));
            }
            
            var buttonTypeAnswers = new ButtonTypeAnswer { Answer = clientAnswer, AnswersCollection = buttons.ToArray()};
            if (!AnswerType.Button.IsCorrect(buttonTypeAnswers))
            {
                var errorMessage = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return this.CurrentStep();
            }

            var answerTag = await _messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, clientAnswer);
            var nextStep = _currentStepTextTags.First(n => n.Key == answerTag).Value;
            if (clientAnswer == await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.YesBtn))
                await _botMessageSender.Send(
                    await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GreateBtn), 
                    await _markupHandler.FormReplyKeyboardMarkupFromButtons(null, true, true));
            
            return nextStep;
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 4;

        private async Task<List<string>> GetButtons()
        {
            var buttons = new List<string>();
            foreach (var tag in _currentStepTextTags)
            {
                var translatedBtn = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, tag.Key);
                buttons.Add(translatedBtn);
            }

            return buttons;
        }
    }
}
