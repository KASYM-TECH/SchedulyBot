using System.Globalization;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch0;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.Shared.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch1
{
    public class S0_ChangeTimeOnActionStep : IDialogStep
    {
        private string _messageTag = "branch1step0mess";
        private string _errorMessage = TextTag.BtnErr;
        private string _allTimeReservedTag = "allTimeReserved";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly UpdateContainer _updateContainer;
        private readonly IScheduleService _scheduleService;

        public S0_ChangeTimeOnActionStep(
            IMessageTranslationManger messageTranslationManger,
            IBotMessageManager botMessageSender,
            UpdateContainer updateContainer,
            IBookingService bookingService, 
            IScheduleService scheduleService)
        {
            _updateContainer = updateContainer;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _bookingService = bookingService;
            _scheduleService = scheduleService;
        }
        
        public async Task SendReplyToUser()
        {
            await _botMessageSender.DeleteInlineKeyboard(_updateContainer.Update.Message.MessageId);
            var bookingForChange = (await _bookingService.GetAllByExecutorId(_updateContainer.Client!.Id))
                .FirstOrDefault(x => x.Status == BookingCompletionStatus.OnChangeTime);

            var currentDate = DateTime.UtcNow.Date;
            var vacantTimes = await GetVacantTimes(currentDate, bookingForChange);
            
            var message =
                await _messageTranslationManger.GetTextByTag(_updateContainer.Template.TranslationCollectionName,
                    _messageTag);
            var markup = CalendarWithOptionsKeyboard.Launch(currentDate, vacantTimes);
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var booking = (await _bookingService.GetAllByExecutorId(_updateContainer.Client!.Id))
                .FirstOrDefault(x => x.Status == BookingCompletionStatus.OnChangeTime);

            var callbackData = _updateContainer.Update.CallbackQuery.Data;
            var splittedData = callbackData.Split(" ");
            if (splittedData.Length == 2)
            {
                var choosedDate = DateTime.Parse(splittedData[0]);
                var choosedTime = DateTime.Parse(splittedData[1]);

                booking.Date = choosedDate;
                booking.BookTimeFrom = choosedTime;
                var endTime = choosedTime.TimeOfDay + booking.ServiceAndSpec.Duration;
                booking.BookTimeTo = DateTime.Parse(endTime.ToString());
                
                await _bookingService.Update(booking);

                return new DialogIteration(TemplateId, 0, 3);
            }
            
            var dateTime = DateTime.Parse(callbackData, CultureInfo.InvariantCulture);
            
            var vacantTimes = await GetVacantTimes(dateTime, booking);
            var message =
                await _messageTranslationManger.GetTextByTag(_updateContainer.Template.TranslationCollectionName,
                    _messageTag);
            
            var markup = CalendarWithOptionsKeyboard.Launch(dateTime, vacantTimes);
            await _botMessageSender.EditInlineKeyboard(_updateContainer.Update.Message.MessageId, message, markup);
            
            return this.CurrentStepWithoutMessage();
        }
        
        private async Task<List<InlineKeyboardButton>> GetVacantTimes(DateTime date, Booking booking)
        {
            var serviceAndSpec = booking.ServiceAndSpec;

            var schedule = await _scheduleService.GetByClientIdAndWeekday(booking.Executor.Id, date.DayOfWeek);
            if (schedule == null)
                return new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations,
                            _allTimeReservedTag),
                        "blank")
                };

            var otherBookings = (await _bookingService
                .GetBookingsByExecutorIdAndDate(booking.Executor.Id, date))
                .Where(x => x.Status == BookingCompletionStatus.Confirmed);

            var vacantTimes = new List<InlineKeyboardButton>();
            var startTime = schedule.TimeFrom.TimeOfDay;
            var endTime = schedule.TimeTo.TimeOfDay;
            var breakStartTime = schedule.BreakTimeFrom.TimeOfDay;
            var breakEndTime = schedule.BreakTimeTo.TimeOfDay;

            var serviceDuration = serviceAndSpec.Duration;
            var vacantTime = startTime.ToString();
            while (TimeSpan.Parse(vacantTime) + serviceDuration <= endTime)
            {
                var startOfVacantTime = TimeSpan.Parse(vacantTime);
                
                var endOfVacantTime = startOfVacantTime + serviceDuration;
                // Сразу итерируем, чтобы continue не убирал нас в вечный цикл
                vacantTime = endOfVacantTime.ToString();
                
                // TODO: Тут работа с UTC
                if (date.Date == DateTime.Now.Date && startOfVacantTime < DateTime.Now.TimeOfDay)
                    continue;

                // Обходим перерыв
                if (startOfVacantTime >= breakStartTime && startOfVacantTime < breakEndTime)
                    continue;
                if (endOfVacantTime > breakStartTime && endOfVacantTime <= breakEndTime)
                    continue;

                // Обходим другие расписания
                var alreadyBooked = false;
                foreach (var otherBooking in otherBookings)
                {
                    alreadyBooked = true;
                    if (startOfVacantTime >= otherBooking.BookTimeFrom.TimeOfDay &&
                        startOfVacantTime < otherBooking.BookTimeTo.TimeOfDay)
                        break;

                    if (endOfVacantTime > otherBooking.BookTimeFrom.TimeOfDay &&
                        endOfVacantTime <= otherBooking.BookTimeTo.TimeOfDay)
                        break;
                    alreadyBooked = false;
                }

                if (alreadyBooked)
                    continue;

                // Наконец добавляем свободное время в список
                var stringOfVacantTime = DateTime.Parse(startOfVacantTime.ToString()).ToString("HH:mm");
                vacantTimes.Add(InlineKeyboardButton.WithCallbackData(stringOfVacantTime,
                    $"{DateOnly.FromDateTime(date).ToString()} {startOfVacantTime}"));
                vacantTime = (startOfVacantTime + serviceDuration).ToString();
            }

            if (!vacantTimes.Any())
                return new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations,
                            _allTimeReservedTag),
                        "blank")
                };

            return vacantTimes;
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int) TemplateEnum.BookingManagmentTemplate;
        public int BranchId { get; } = 1;
        public int StepId { get; } = 0;
    }
}
