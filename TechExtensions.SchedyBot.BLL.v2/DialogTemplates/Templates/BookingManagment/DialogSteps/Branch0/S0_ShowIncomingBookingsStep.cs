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

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch0
{
    public class S0_ShowIncomingBookingsStep : IDialogStep
    {
        // TODO: Заменить  англ описание
        private const string _messageTag = "branch0step0mess";
        private string _allTimeReservedTag = "allTimeReserved";
        private const string _errorMessage = TextTag.BtnErr;

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly UpdateContainer _updateContainer;
        private UpdateContainer _container;


        public S0_ShowIncomingBookingsStep(
            IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageSender,
            UpdateContainer container,
            UpdateContainer updateContainer,
           IBookingService bookingService
           )
        {
            _container = container;
            _updateContainer = updateContainer;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _bookingService = bookingService;
        }
        
        public async Task SendReplyToUser()
        {
            var currentDate = DateTime.Now.Date;

            var message =
                await _messageTranslationManger.GetTextByTag(
                    _updateContainer.Template.TranslationCollectionName,
                    _messageTag);
            var listOfButtons = await GetButtons(currentDate);
            var markup = CalendarWithOptionsKeyboard.Launch(currentDate, listOfButtons, false);
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var callbackData = _updateContainer.Update.CallbackQuery.Data;

            var splittedData = callbackData.Split(" ");
            if (splittedData.Length == 3)
            {
                var bookingId = callbackData!.Split(" ")[2];
                _container.Update.Message!.Text = "executorId " + bookingId;
                return new DialogIteration(TemplateId, 0, 2);
            }
            
            var dateTime = DateTime.Parse(callbackData, CultureInfo.InvariantCulture);
            var message =
                await _messageTranslationManger.GetTextByTag(_updateContainer.Template.TranslationCollectionName,
                    _messageTag);

            var listOfButtons = await GetButtons(dateTime);
            var markup = CalendarWithOptionsKeyboard.Launch(dateTime, listOfButtons, false);
            await _botMessageSender.EditInlineKeyboard(_updateContainer.Update.Message.MessageId, message, markup);

            return this.CurrentStepWithoutMessage();
        }

        private async Task<List<InlineKeyboardButton>> GetButtons(DateTime date)
        {
            var allBookings = (await _bookingService
                .GetAllByExecutorIdAndDate(_updateContainer.Client.Id, date))
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
                var clientName = booking.Client.FirstName + " " + booking.Client.LastName;

                var buttonTitle = $"[{serviceName}] {clientName} ({time})";
                if (booking.Status == BookingCompletionStatus.AwaitingConfirmation)
                    buttonTitle += " 🆕";
                
                var callbackData = $"{TelegramCommand.IncomingBookings} {BookingAction.info} {booking.Id}";
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
        public int BranchId { get; } = 0;
        public int StepId { get; } = 0;
    }
}
