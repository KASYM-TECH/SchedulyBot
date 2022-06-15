using System.Globalization;
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
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch2
{
    public class S0_SelectOutgoingBookingStep : IDialogStep
    {
        private const string _messageForSellerTag = "branch0step0mess";
        private const string _messageToSelectBookingTag = "branch0step1mess";
        private string _allTimeReservedTag = "allTimeReserved";
        private const string _errorMessage = TextTag.BtnErr;

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private UpdateContainer _container;
        private readonly UpdateContainer _updateContainer;
        

        public S0_SelectOutgoingBookingStep(
            IMessageTranslationManger messageTranslationManger,
           IBookingService bookingService, 
            UpdateContainer updateContainer, 
            IBotMessageManager botMessageSender)
        {
            _messageTranslationManger = messageTranslationManger;
            _bookingService = bookingService;
            _updateContainer = updateContainer;
            _botMessageSender = botMessageSender;
        }
        
        public async Task SendReplyToUser()
        {
            var currentDate = DateTime.Now.Date;
            var message =
                await _messageTranslationManger.GetTextByTag(
                    _updateContainer.Template!.TranslationCollectionName,
                    _messageForSellerTag);
            var listOfButtons = await GetButtons(currentDate);
            var markup = CalendarWithOptionsKeyboard.Launch(currentDate, listOfButtons, false);
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var booking = await _bookingService.GetNewByClientId(_updateContainer.Client!.Id);
            var callbackData = _updateContainer.Update.CallbackQuery.Data;

            var splittedData = callbackData.Split(" ");
            if (splittedData.Length == 3)
            {
                var bookingId = callbackData!.Split(" ")[2];
                _container.Update.Message!.Text = "executorId " + bookingId;
                return this.NextStep();
            }

            var dateTime = DateTime.Parse(callbackData, CultureInfo.InvariantCulture);
            var message =
                await _messageTranslationManger.GetTextByTag(_updateContainer.Template.TranslationCollectionName,
                    _messageForSellerTag);

            var listOfButtons = await GetButtons(dateTime);
            var markup = CalendarWithOptionsKeyboard.Launch(dateTime, listOfButtons, false);
            await _botMessageSender.EditInlineKeyboard(_updateContainer.Update.Message.MessageId, message, markup);

            return this.CurrentStepWithoutMessage();
        }

        private async Task<List<InlineKeyboardButton>> GetButtons(DateTime date)
        {
            var allBookings = (await _bookingService
                .GetAllByClientIdAndDate(_updateContainer.Client.Id, date))
                .Where(x => x.Status != BookingCompletionStatus.Created &&
                            x.Status != BookingCompletionStatus.New &&
                            x.Status != BookingCompletionStatus.Canceled &&
                            x.Status != BookingCompletionStatus.Rejected);

            var listOfButtons = new List<InlineKeyboardButton>();
            foreach (var booking in allBookings.OrderBy(x => x.BookTimeFrom.TimeOfDay))
            {
                var serviceName = booking.ServiceAndSpec.Service.Name;
                var time = booking.BookTimeFrom.ToShortTimeString();
                // TODO: Вынести в метод внутри клиента типа "Дай мне полное имя" или в экстеншн с тем же смыслом
                var clientName = booking.Executor.FirstName + " " + booking.Executor.LastName;

                var buttonTitle = $"[{serviceName}] {clientName} ({time})";
                if (booking.Status == BookingCompletionStatus.AwaitingConfirmation)
                    buttonTitle += " 🆕";

                var callbackData = $"{TelegramCommand.OutgoingBookings} {BookingAction.info} {booking.Id}";
                var inlineButton = InlineKeyboardButton.WithCallbackData(buttonTitle, callbackData);

                listOfButtons.Add(inlineButton);
            }

            if (!listOfButtons.Any())
                listOfButtons = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations,
                            _allTimeReservedTag),
                        "blank")
                };

            return listOfButtons;
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int) TemplateEnum.BookingManagmentTemplate;
        public int BranchId { get; } = 2;
        public int StepId { get; } = 0;
    }
}
