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
    public class S1_InputNameStep : IDialogStep
    {
        private string _messageTag = "branch1step1mess";
        private string _errorTag = "branch1EnterErr";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageManager;
        private readonly IClientService _clientService;
        private readonly UpdateContainer _container;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;

        public S1_InputNameStep(
            IMessageTranslationManger messageTranslationManger,
            IClientService clientService,
            IBotMessageManager botMessageManager,
            UpdateContainer container,
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _messageTranslationManger = messageTranslationManger;
            _clientService = clientService;
            _botMessageManager = botMessageManager;
            _container = container;
            _markupHandler = markupHandler;
        }

        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            var buttons = new List<string>
                { _container.User!.FirstName, _container.User!.LastName ?? "" }
                .Where(x => !x.IsNullOrEmpty())
                .ToList();

            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(buttons, true, true);
            await _botMessageManager.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            if (!AnswerType.Name.IsCorrect(clientAnswer))
            {
                var message =
                    await _messageTranslationManger.GetTextByTag(
                        _container.Template!.TranslationCollectionName, _errorTag);
                await _botMessageManager.Send(message);

                return this.CurrentStep();
            }

            _container.Client!.FirstName = clientAnswer;
            await _clientService.Update(_container.Client);
            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int)TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 1;
        public int StepId { get; } = 1;
    }
}
