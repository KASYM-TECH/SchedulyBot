using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.Shared.Extensions;


namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch1
{
    public class S4_NowYouCanBookTimeStep : IDialogStep
    {
        private string _messageTag = "branch1step3mess";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageManager;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        private readonly UpdateContainer _container;
        private readonly IBookingService _bookingService;
        private readonly IClientService _clientService;

        public S4_NowYouCanBookTimeStep(
            IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageManager,
            UpdateContainer container,
            IBookingService bookingService,
            IClientService clientService,
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _messageTranslationManger = messageTranslationManger;
            _botMessageManager = botMessageManager;
            _container = container;
            _bookingService = bookingService;
            _clientService = clientService;
            _markupHandler = markupHandler;
        }
        public async Task<DialogIteration> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            if (_messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, clientAnswer).Result != TextTag.GreateBtn)
            {
                var message = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.BtnErr);
                await _botMessageManager.Send(message);
                return this.CurrentStep();
            }
            return new DialogIteration(TemplateId, 0, 1);
        }

        public async Task SendReplyToUser()
        {
            var btn = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GreateBtn);
            var message = await _messageTranslationManger.GetTextByTag(CollectionName.BookSellerTimeTemplate, _messageTag);
            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(new List<string> { btn }, true, true);
            var booking = await _bookingService.GetNewByClientId(_container.Client!.Id);
            var formattedMessage = String.Format(message, booking!.Executor.GetFullName());
            await _botMessageManager.Send(formattedMessage, markup);
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int)TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 1;
        public int StepId { get; } = 4;
    }
}
