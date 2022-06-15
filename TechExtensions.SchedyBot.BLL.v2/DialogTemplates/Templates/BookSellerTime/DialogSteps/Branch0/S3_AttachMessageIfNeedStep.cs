using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;


namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch0;

public class S3_AttachMessageIfNeedStep : IDialogStep
    {
        private string _messageTag = "branch0step3mess";
        private string _btnTag = "branch0step3btn1";
        
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly UpdateContainer _container;
        private readonly IBookingService _bookingService;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        
        public S3_AttachMessageIfNeedStep(
           IMessageTranslationManger messageTranslationManger,
           IBotMessageManager botMessageSender,
           IBookingService bookingService, 
           UpdateContainer container, 
           BotReplyKeyboardMarkupHandler markupHandler)
        {
            _bookingService = bookingService;
            _container = container;
            _markupHandler = markupHandler;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }

        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(_container.Template.TranslationCollectionName, _messageTag);
            var buttons = new List<string>
            {
                await _messageTranslationManger.GetTextByTag(_container.Template.TranslationCollectionName, _btnTag)
            };
            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(buttons, true, true);
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var booking = await _bookingService.GetNewByClientId(_container.Client.Id);
            if (await _messageTranslationManger.GetTextByTag(_container.Template.TranslationCollectionName, _btnTag) !=
                clientAnswer)
                booking.MessageForExecutor = clientAnswer;

            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int) TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 3;
}