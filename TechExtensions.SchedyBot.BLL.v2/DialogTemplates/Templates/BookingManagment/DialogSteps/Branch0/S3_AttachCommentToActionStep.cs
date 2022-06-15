using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch0
{
    public class S3_AttachCommentToActionStep : IDialogStep
    {
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly UpdateContainer _updateContainer;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;

        private string _btnTag = "branch0step3btn";
        private string _messageAboutComment = "branch0step3mess";
        

        public S3_AttachCommentToActionStep(
            IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageSender,
           UpdateContainer updateContainer,
           IBookingService bookingService, 
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _updateContainer = updateContainer;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _bookingService = bookingService;
            _markupHandler = markupHandler;
        }

        public async Task SendReplyToUser()
        {
            await _botMessageSender.DeleteInlineKeyboard(_updateContainer.Update.Message.MessageId);
            var message = await _messageTranslationManger.GetTextByTag(_updateContainer.Template.TranslationCollectionName, _messageAboutComment);
            var buttons = new List<string>
            {
                await _messageTranslationManger.GetTextByTag(_updateContainer.Template.TranslationCollectionName, _btnTag)
            };
            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(buttons, true, true);
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var booking = (await _bookingService.GetAllByExecutorId(_updateContainer.Client.Id))
                .FirstOrDefault(x => x.Status == BookingCompletionStatus.OnConfirm ||
                                     x.Status == BookingCompletionStatus.OnReject ||
                                     x.Status == BookingCompletionStatus.OnChangeTime);
            if (await _messageTranslationManger.GetTextByTag(_updateContainer.Template.TranslationCollectionName, _btnTag) !=
                clientAnswer)
                booking.MessageForClient = clientAnswer;
            
            await _bookingService.Update(booking);

            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int) TemplateEnum.BookingManagmentTemplate;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 3;

    }
}
