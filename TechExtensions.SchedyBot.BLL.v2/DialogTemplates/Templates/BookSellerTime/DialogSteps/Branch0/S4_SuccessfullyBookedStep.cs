using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.Enums;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.Shared.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch0
{
    public class S4_SuccessfullyBookedStep : IDialogStep
    {
        private string _messageForExecutorTag = "branch0step3mess3";
        private string _confirmTag = "confirm";
        private string _rejectTag = "rejectBtn";
        private string _changeTimeTag = "changeTime";
        private string _messageForClientTag = "branch0step3mess2";
        private string _commentTag = "comment";

        private string _errorMessage = TextTag.BtnErr;

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly UpdateContainer _container;
        private readonly IBookingService _bookingService;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;


        public S4_SuccessfullyBookedStep(
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
        public Task<DialogIteration> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            throw new NotImplementedException();
        }

        public async Task SendReplyToUser()
        {
            var booking = await _bookingService.GetNewByClientId(_container.Client.Id);
            // Сперва отправим сообщение исполнителю, потом заказчику
            var messageForExecutor =
                await _messageTranslationManger.GetTextByTag(_container.Template.TranslationCollectionName,
                    _messageForExecutorTag);
            var formattedMessageForExecutor = string.Format(
                messageForExecutor,
                booking!.Client.FirstName + " " + booking.Client.LastName,
                booking.ServiceAndSpec.Service.Name,
                booking.Date.ToShortDateString(),
                booking.BookTimeFrom.ToShortTimeString(),
                TelegramCommand.IncomingBookings);

            var buttons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _confirmTag),
                        $"{TelegramCommand.IncomingBookings} {BookingAction.confirm} {booking.Id}"),
                    InlineKeyboardButton.WithCallbackData(
                        await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _rejectTag),
                        $"{TelegramCommand.IncomingBookings} {BookingAction.reject} {booking.Id}"),
                },
                new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _changeTimeTag),
                        $"{TelegramCommand.IncomingBookings} {BookingAction.change} {booking.Id}"),
                }
            };

            var markup = await _markupHandler.FormInlineKeyboardMarkupFromButtons(buttons);
            var executorChatId = booking.Executor.ChatId;

            var comment = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _commentTag, booking.Executor.Language);
            if (!booking.MessageForExecutor.IsNullOrEmpty())
                formattedMessageForExecutor += "\n" + string.Format(comment, _container.Update.Message!.Text);

            await _botMessageSender.Send(formattedMessageForExecutor, markup, executorChatId);

            var messageForClient =
                await _messageTranslationManger.GetTextByTag(_container.Template.TranslationCollectionName,
                    _messageForClientTag);
            await _botMessageSender.Send(messageForClient);

            booking.Status = BookingCompletionStatus.AwaitingConfirmation;
            await _bookingService.Update(booking);
        }
        public bool LastStep { get; set; } = true;
        public int TemplateId { get; } = (int)TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 4;
    }
}
