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
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch1
{
    public class S0_QuickRegistrationStarts : IDialogStep
    {
        private string _messageTag = "branch1step0mess";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageManager;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        private readonly IClientService _clientService;
        private readonly UpdateContainer _container;
        private readonly IBookingService _bookingService;

        public S0_QuickRegistrationStarts(
            IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageManager,
            IClientService clientService,
            IBookingService bookingService,
            UpdateContainer container,
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _messageTranslationManger = messageTranslationManger;
            _container = container;
            _bookingService = bookingService;
            _clientService = clientService;
            _botMessageManager = botMessageManager;
            _markupHandler = markupHandler;
        }

        public async Task SendReplyToUser()
        {
            var startParameter = _container.Update.Message!.Text!.Split(' ')[1];
            var executorId = int.Parse(startParameter.Replace("executorId", ""));
            var executor = await _clientService.GetById(executorId);

            var booking = new Booking
            {
                ClientId = _container.Client!.Id,
                ExecutorId = executorId,
                Status = BookingCompletionStatus.New
            };
            await _bookingService.Create(booking);

            var btn = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GreateBtn);
            var message = await _messageTranslationManger.GetTextByTag(CollectionName.BookSellerTimeTemplate, _messageTag);
            var formattedMessage = String.Format(message, executor.GetFullName());
            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(new List<string> { btn }, true, true);
            await _botMessageManager.Send(formattedMessage, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            if (_messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, clientAnswer).Result != TextTag.GreateBtn)
            {
                var message = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.BtnErr);
                await _botMessageManager.Send(message);
                return this.CurrentStep();
            }
            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int)TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 1;
        public int StepId { get; } = 0;
    }
}
