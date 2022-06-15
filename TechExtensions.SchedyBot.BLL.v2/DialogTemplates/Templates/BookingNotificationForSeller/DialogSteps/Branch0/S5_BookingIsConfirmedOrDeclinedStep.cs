using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch0
{
    public class S5_BookingIsConfirmedOrDeclinedStep : IDialogStep
    {
        private string _messageBookingConfirmedTag = "branch0step5mess1";
        private string _messageBookingDeclinedTag = "branch0step5mess2";
        private string _messageNotifyClientOfChangeInTimeTag = "branch0step5mess3";
        private string _messageNotifyClientOfCanceledBooking = "branch0step5mess4";
        private string _messageNotifyClientOfConfirmedBooking = "branch0step5mess5";

        public Dictionary<Type, string> NextStepTextTag => throw new NotImplementedException();

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBookingInConfirmationService _bookingInConfirmationService;
        private readonly IBotMessageManager _botMessageSender; 
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly IServiceKindService _serviceKindService;
        private readonly IServiceService _serviceService;
        private readonly IBookingService _bookingService;
        public S5_BookingIsConfirmedOrDeclinedStep(IMessageTranslationManger messageTranslationManger,
            IBookingService bookingService,
            IServiceService serviceService,
            IServiceAndSpecService serviceAndSpecService,
            IServiceKindService serviceKindService,
            IBookingInConfirmationService bookingInConfirmationService,
            IBotMessageManager botMessageSender)
        {
            _serviceService = serviceService;
            _bookingService = bookingService;
            _serviceAndSpecService = serviceAndSpecService;
            _serviceKindService = serviceKindService;
            _bookingInConfirmationService = bookingInConfirmationService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }

        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var s1 = _serviceAndSpecService.GetManyServiceAndSpec(s => s.IsActive);
            var s2 = _serviceKindService.GetServices(s => s.IsActive);
            var s3 = _serviceService.GetServices(s => s.IsActive);
            var bookingUnderSellerConsideration = _bookingInConfirmationService.Get(b => b.BookingToConfirm.Status == BookingCompletionStatus.IsInConfirmingTemplate
                           && b.BookingToConfirm.Executor.Id == currentDialog.Client.Id);
            var message = await GetMessage(currentDialog, translationCollectionName, bookingUnderSellerConsideration!);
            currentDialog.State = CurrentDialogState.Suspended;
            await _botMessageSender.Send(message);
            await _botMessageSender.SendNeutralMessage();
            bookingUnderSellerConsideration!.BookingToConfirm.MessageForClient = bookingUnderSellerConsideration.MessageForClient;
            if (bookingUnderSellerConsideration!.Status == BookingCompletionStatusDuringSellerConfirmation.IsBeingConfirmed)
                await HandleConfirmationByExecutor(bookingUnderSellerConsideration, translationCollectionName);
            else if (bookingUnderSellerConsideration.Status == BookingCompletionStatusDuringSellerConfirmation.IsBeingCanceled)
                await HandleCacelationByExecutor(bookingUnderSellerConsideration, translationCollectionName);
            else if (bookingUnderSellerConsideration.Status == BookingCompletionStatusDuringSellerConfirmation.TimeIsBeingChanged)
                await HandleChangingTimeByExecutor(bookingUnderSellerConsideration, translationCollectionName);
            await _bookingService.Update(bookingUnderSellerConsideration.BookingToConfirm);
            bookingUnderSellerConsideration.BookingToConfirm = null;
            await _bookingInConfirmationService.Delete(bookingUnderSellerConsideration);
            currentDialog.CurrentStepType = null;
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            throw new NotImplementedException();
        }

        public int TemplateId { get; } = 3;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 5;
        
        private async Task HandleConfirmationByExecutor(BookingInConfirmation bookingUnderSellerConsideration, string translationCollectionName)
        {
            bookingUnderSellerConsideration.BookingToConfirm.Status = BookingCompletionStatus.Confirmed;
            var serviceName = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.ServiceCollection, bookingUnderSellerConsideration.BookingToConfirm.ServiceAndSpec.Service.NameTag);
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageNotifyClientOfConfirmedBooking);
            var replacedMessage = message
                .Replace("{seller.name}", bookingUnderSellerConsideration.BookingToConfirm.Executor.FirstName)
                .Replace("{date}", bookingUnderSellerConsideration.BookingToConfirm.Date.Date.ToString())
                .Replace("{service.name}", serviceName)
                .Replace("{duration.From}", bookingUnderSellerConsideration.BookingToConfirm.BookTimeFrom.ToShortTimeString())
                .Replace("{service.price}", bookingUnderSellerConsideration.BookingToConfirm.ServiceAndSpec.Price.ToString())
                .Replace("{duration.To}", bookingUnderSellerConsideration.BookingToConfirm.BookTimeTo.ToShortTimeString());
            await _botMessageSender.Send(replacedMessage, chatId: bookingUnderSellerConsideration.BookingToConfirm.User.ChatId);
        }
        private async Task HandleCacelationByExecutor(BookingInConfirmation bookingUnderSellerConsideration, string translationCollectionName)
        {
            bookingUnderSellerConsideration.BookingToConfirm.Status = BookingCompletionStatus.CanceledBySeller;
            var serviceName = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.ServiceCollection, bookingUnderSellerConsideration.BookingToConfirm.ServiceAndSpec.Service.NameTag);
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageNotifyClientOfCanceledBooking);
            var replacedMessage = message
                .Replace("{seller.name}", bookingUnderSellerConsideration.BookingToConfirm.Executor.FirstName)
                .Replace("{service.name}", serviceName)
                .Replace("{duration.From}", bookingUnderSellerConsideration.BookingToConfirm.BookTimeFrom.ToShortTimeString())
                .Replace("{duration.To}", bookingUnderSellerConsideration.BookingToConfirm.BookTimeTo.ToShortTimeString());
            await _botMessageSender.Send(replacedMessage, chatId: bookingUnderSellerConsideration.BookingToConfirm.User.ChatId);
        }
        private async Task HandleChangingTimeByExecutor(BookingInConfirmation bookingUnderSellerConsideration, string translationCollectionName)
        {
            bookingUnderSellerConsideration.Status =  BookingCompletionStatusDuringSellerConfirmation.TimeIsBeingChanged;
            var serviceName = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.ServiceCollection, bookingUnderSellerConsideration.BookingToConfirm.ServiceAndSpec.Service.NameTag);
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageNotifyClientOfChangeInTimeTag);
            var replacedMessage = message
                .Replace("{seller.name}", bookingUnderSellerConsideration.BookingToConfirm.Executor.FirstName)
                .Replace("{date}", bookingUnderSellerConsideration.BookingToConfirm.Date.Date.ToString())
                .Replace("{service.name}", serviceName)
                .Replace("{duration.From}", bookingUnderSellerConsideration.BookingToConfirm.BookTimeFrom.ToShortTimeString())
                .Replace("{duration.To}", bookingUnderSellerConsideration.BookingToConfirm.BookTimeTo.ToShortTimeString());
            await _botMessageSender.Send(replacedMessage, chatId: bookingUnderSellerConsideration.BookingToConfirm.User.ChatId);
        }
        public async Task<string> GetMessage(CurrentDialog currentDialog, string translationCollectionName, BookingInConfirmation booking)
        {
            var messageTag = _messageBookingConfirmedTag;
            if (booking.Status == BookingCompletionStatusDuringSellerConfirmation.IsBeingCanceled)
                messageTag = _messageBookingDeclinedTag;
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, messageTag);
            return message;
        }
    }
}
