using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.Shared.Extensions;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch2
{
    public class S3_OutcomeBookingActionHandleStep : IDialogStep
    {
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly UpdateContainer _updateContainer;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        private readonly HangFireService _hangFireService;

        private string _btnTag = "branch0step3btn";
        private string _messageAboutComment = "branch0step3mess";

        private string _onCanceledExecutorTag = "branch0step3mess2";
        private string _onCanceledClientTag = "branch0step3mess7";
        private string _onChangeTimeExecutorTag = "branch0step3mess3";
        private string _onChangeTimeClientTag = "branch0step3mess6";
        private string _notificationMessageForClient = "branch0step3mess8";
        private string _commentTag = "comment";

        private string _errorMessage = TextTag.BtnErr;

        public S3_OutcomeBookingActionHandleStep(
            IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageSender,
            UpdateContainer updateContainer,
            IBookingService bookingService,
            HangFireService hangFireService,
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _updateContainer = updateContainer;
            _hangFireService = hangFireService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _bookingService = bookingService;
            _markupHandler = markupHandler;
        }
        public async Task SendReplyToUser()
        {
            var booking = (await _bookingService.GetAllByClientId(_updateContainer.Client.Id))
                .FirstOrDefault(x => x.Status == BookingCompletionStatus.OnCancel ||
                                     x.Status == BookingCompletionStatus.OnChangeTime);
            var collection = _updateContainer.Template.TranslationCollectionName;

            var onChangeTimeExecutor = string.Format(
            await _messageTranslationManger.GetTextByTag(collection, _onChangeTimeExecutorTag, booking.Executor.Language),
            booking.Client.GetFullName(),
            booking.ServiceAndSpec.Service.Name,
            booking.Date.ToShortDateString(),
            booking.BookTimeFrom.ToShortTimeString());

            var onCanceledExecutor = string.Format(
            await _messageTranslationManger.GetTextByTag(collection, _onCanceledExecutorTag, booking.Executor.Language),
            booking.Client.GetFullName(),
            booking.ServiceAndSpec.Service.Name,
            booking.Date.ToShortDateString(),
            booking.BookTimeFrom.ToShortTimeString());

            var onCanceledClient = string.Format(
            await _messageTranslationManger.GetTextByTag(collection, _onCanceledClientTag, booking.Client.Language),
            booking.Executor.GetFullName(),
            booking.ServiceAndSpec.Service.Name,
            booking.Date.ToShortDateString(),
            booking.BookTimeFrom.ToShortTimeString());

            var onChangeTimeClient = string.Format(
            await _messageTranslationManger.GetTextByTag(collection, _onChangeTimeClientTag, booking.Client.Language),
            booking.Executor.GetFullName(),
            booking.ServiceAndSpec.Service.Name,
            booking.Date.ToShortDateString(),
            booking.BookTimeFrom.ToShortTimeString());

            var notificationMessageForClient = await _messageTranslationManger.GetTextByTag(collection, _notificationMessageForClient, booking.Client.Language);
            var formattedNotificationMessageForClient = String.Format(notificationMessageForClient, booking.Executor.GetFullName(),
                booking.BookTimeFrom.ToShortTimeString(), booking.ServiceAndSpec.Service.Name);

            var messageForExecutor = String.Empty;
            var messageForClient = String.Empty;
            switch (booking.Status)
            {
                case BookingCompletionStatus.OnCancel:
                    booking.Status = BookingCompletionStatus.Canceled;
                    messageForExecutor = onCanceledExecutor;
                    messageForClient = onCanceledClient;
                    if(booking.Status == BookingCompletionStatus.Confirmed)
                        _hangFireService.DeleteScheduledTime(booking.HangFireJobId);
                    break;
                case BookingCompletionStatus.OnChangeTime:
                    booking.Status = BookingCompletionStatus.Confirmed;
                    messageForExecutor = onChangeTimeExecutor;
                    messageForClient = onChangeTimeClient;
                    if (booking.Status == BookingCompletionStatus.Confirmed)
                    {
                        _hangFireService.DeleteScheduledTime(booking.HangFireJobId);
                        _hangFireService.ScheduleTime(booking.BookTimeFrom, booking.Client.TimeZoneOffset, booking.Client.ChatId, formattedNotificationMessageForClient);
                    }
                    break;
            }

            var comment = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _commentTag);
            if (!booking.MessageForClient.IsNullOrEmpty())
                messageForExecutor += "\n" + string.Format(comment, _updateContainer.Update.Message!.Text);

            await _bookingService.Update(booking);
            await _botMessageSender.Send(messageForClient, null, booking.Client.ChatId);
            await _botMessageSender.Send(messageForExecutor, null, booking.Executor.ChatId);
        }
        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            throw new NotImplementedException();
        }
        public bool LastStep { get; set; } = true;
        public int TemplateId { get; } = (int)TemplateEnum.BookingManagmentTemplate;
        public int BranchId { get; } = 2;
        public int StepId { get; } = 3;
    }
}
