using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.Shared.Extensions;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch1
{
    public class S2_InputLastNameStep : IDialogStep
    {
        private string _messageTag = "branch1step2mess";
        private string _errorTag = "branch1EnterErr";
        private string _skipButtonTag = "branch1step2btn1";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageManager;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        private readonly UpdateContainer _container;
        private readonly IClientService _clientService;

        public S2_InputLastNameStep(
            IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageManager,
            UpdateContainer container,
            IClientService clientService,
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _messageTranslationManger = messageTranslationManger;
            _botMessageManager = botMessageManager;
            _container = container;
            _clientService = clientService;
            _markupHandler = markupHandler;
        }

        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            var messageWithUserName = string.Format(message, _container.Client!.FirstName);

            var skipButtonText = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _skipButtonTag);
            var buttons = new List<string>
                    { _container.User!.FirstName, _container.User!.LastName ?? "", skipButtonText }
                .Where(x => !x.IsNullOrEmpty())
                .ToList();

            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(buttons, true, true);
            await _botMessageManager.Send(messageWithUserName, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var skipButtonText = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _skipButtonTag);
            if (skipButtonText == clientAnswer)
                return this.NextStep();

            if (!AnswerType.Name.IsCorrect(clientAnswer))
            {
                var message =
                    await _messageTranslationManger.GetTextByTag(
                        _container.Template!.TranslationCollectionName, _errorTag);
                await _botMessageManager.Send(message);

                return this.CurrentStep();
            }

            _container.Client!.LastName = clientAnswer;
            await _clientService.Update(_container.Client);

            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int)TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 1;
        public int StepId { get; } = 2;
    }
}
