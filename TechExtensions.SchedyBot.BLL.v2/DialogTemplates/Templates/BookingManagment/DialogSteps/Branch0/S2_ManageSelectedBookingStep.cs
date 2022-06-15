using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.Enums;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.Shared.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch0
{
    public class S2_ManageSelectedBookingStep : IDialogStep
    {
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly UpdateContainer _updateContainer;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;

        private string _confirmTag = "confirm";
        private string _rejectTag = "rejectBtn";
        private string _changeTimeTag = "changeTime";
        private string _messageForExecutorTag = "branch0step3mess1";
        
        private string _commentTag = "comment";
        
        private string _errorMessage = TextTag.BtnErr;

        public S2_ManageSelectedBookingStep (
            IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageSender,
           UpdateContainer updateContainer,
           IBookingService bookingService, BotReplyKeyboardMarkupHandler markupHandler)
        {
            _updateContainer = updateContainer;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _bookingService = bookingService;
            _markupHandler = markupHandler;
        }

        public async Task SendReplyToUser()
        {
            var callbackData = _updateContainer.Update.CallbackQuery.Data;
            var parameters = callbackData.Split(" ");
            var bookingId = parameters[2];
            await _botMessageSender.DeleteInlineKeyboard(_updateContainer.Update.Message.MessageId);

            var booking = await _bookingService.GetById(int.Parse(bookingId));
            var listOfBasicActions = new List<InlineKeyboardButton>();
            if (booking.Status == BookingCompletionStatus.AwaitingConfirmation)
                listOfBasicActions.Add(InlineKeyboardButton.WithCallbackData(
                    await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _confirmTag),
                    $"{TelegramCommand.IncomingBookings} {BookingAction.confirm} {booking.Id}"));
            listOfBasicActions.Add(
                InlineKeyboardButton.WithCallbackData(
                    await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _rejectTag),
                    $"{TelegramCommand.IncomingBookings} {BookingAction.reject} {booking.Id}"));
            
            var buttons = new List<List<InlineKeyboardButton>>
            {
                listOfBasicActions,
                new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _changeTimeTag),
                        $"{TelegramCommand.IncomingBookings} {BookingAction.change} {booking.Id}"),
                }
            };
            
            var messageForExecutor =
                await _messageTranslationManger.GetTextByTag(CollectionName.BookSellerTimeTemplate,
                    _messageForExecutorTag);
            var comment = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _commentTag);
            if (!booking.MessageForExecutor.IsNullOrEmpty())
                messageForExecutor += "\n" + string.Format(comment, booking.MessageForExecutor);
            var formattedMessageForExecutor = string.Format(
                messageForExecutor,
                booking.Client.FirstName + " " + booking.Client.LastName,
                booking.ServiceAndSpec.Service.Name,
                booking.Date.ToShortDateString(),
                booking.BookTimeFrom.ToShortTimeString());

            var markup = await _markupHandler.FormInlineKeyboardMarkupFromButtons(buttons);
            await _botMessageSender.Send(formattedMessageForExecutor, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var callbackData = _updateContainer.Update.CallbackQuery.Data;
            var parameters = callbackData.Split(" ");
            var bookingAction = Enum.Parse<BookingAction>(parameters[1]);
            var bookingId = parameters[2];
            var booking = await _bookingService.GetById(int.Parse(bookingId));

            var iteration = this.NextStep();
            switch (bookingAction)
            {
                case BookingAction.confirm:
                    booking.Status = BookingCompletionStatus.OnConfirm;
                    break;
                case BookingAction.reject:
                    booking.Status = BookingCompletionStatus.OnReject;
                    break;
                case BookingAction.change:
                    booking.Status = BookingCompletionStatus.OnChangeTime;
                    iteration = this.NextBranch();
                    break;
            }

            await _bookingService.Update(booking);

            return iteration;
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int) TemplateEnum.BookingManagmentTemplate;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 2;
    }
}
