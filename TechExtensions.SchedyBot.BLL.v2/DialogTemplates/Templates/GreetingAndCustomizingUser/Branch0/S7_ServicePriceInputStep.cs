using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;
using DialogStepExtensions = TechExtensions.SchedyBot.BLL.v2.Extensions.DialogStepExtensions;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0
{
    public class S7_ServicePriceInputStep : IDialogStep
    {
        private string _messageTag = "branch0step7mess";
        private string _errorMessage = "branch0step7err";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly UpdateContainer _container;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;

        public S7_ServicePriceInputStep(IMessageTranslationManger messageTranslationManger,
             IBotMessageManager botMessageSender,
           IServiceAndSpecService serviceAndSpecService, BotReplyKeyboardMarkupHandler markupHandler, UpdateContainer container)
        {
            _serviceAndSpecService = serviceAndSpecService;
            _markupHandler = markupHandler;
            _container = container;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }

        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            await _botMessageSender.Send(message, await _markupHandler.FormReplyKeyboardMarkupFromButtons(null, true, true));
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            // TODO: Сделать проверку на Number
            if (!AnswerType.IntegralNumber.IsCorrect(clientAnswer))
            {
                var errorMessage =
                    await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName,
                        _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return this.CurrentStep();
            }

            var serviceAndSpec = await _serviceAndSpecService.GetByClientId(_container.Client!.Id);
            if (serviceAndSpec == null)
                throw new NullReferenceException(
                    "На прошлом шаге создали сервис энд спек, а теперь его не существует, странно.");

            serviceAndSpec.Price = int.Parse(clientAnswer);
            await _botMessageSender.Send(
                await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GreateBtn),
                await _markupHandler.FormReplyKeyboardMarkupFromButtons(null, true, true));
            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 7;
    }
}
