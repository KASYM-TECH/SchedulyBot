using TechExtensions.SchedyBot.BLL.Services;
using TechExtensions.SchedyBot.DLL.Entities;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.BotMessageSender;
using TechExtensions.SchedyBot.BLL.ReservationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.Services.Abstractions;

namespace TechExtensions.SchedyBot.BLL.ReservationManager
{
    public class ReservationManager : IReservationManager
    {
        private readonly IBookingService _bookingService;
        private readonly IBotMessageManager _botMessageSender;

        public ReservationManager(IBookingService bookingService, IBotMessageManager botMessageSender)
        {
            _bookingService = bookingService;
            _botMessageSender = botMessageSender;
        }

        public async Task<Dictionary<DateTime, DateTime>> GetFreeTimeScheduleByDay(Schedule schedule, DateTime bookingDate,
         long executorChatId, TimeSpan assumedDuration, int GapBetweenBookingsInMinutes = 5)
        {
            var duration = assumedDuration.Duration();
            var assumedDurationInMinutes = (duration.Hours * 60) + duration.Minutes;
            var freeBookingTimes = new Dictionary<DateTime, DateTime>();

            //Все bookings в этот день у этого же продавца
            var bookingsOnTheSameDay =  _bookingService.GetBookings(b => b.Date == bookingDate.Date && 
            b.Executor.ChatId == executorChatId);

            //Пробегаемся по всем возможным bookingTimes и проверяем свободно ли оно, если да, добавляем в freeBookingTimes
            for (DateTime bookingFrom = schedule.TimeFrom; bookingFrom.AddMinutes(assumedDurationInMinutes) <= schedule.TimeTo;
                bookingFrom += TimeSpan.FromMinutes(GapBetweenBookingsInMinutes))
            {
                var bookingTo = bookingFrom.AddMinutes(assumedDurationInMinutes);

                var isBookingTimeFree = IsBookingTimeFree(bookingFrom, bookingTo, bookingsOnTheSameDay);

                if (isBookingTimeFree)
                    freeBookingTimes.Add(bookingFrom, bookingTo);
            }

            return freeBookingTimes;
        }

        /// <summary>
        /// удостоверяется что время свободно
        /// </summary>
        /// <param name="assumedBookingTime"></param>
        /// <returns></returns>
        public async Task<bool> TryToConfirmBookingTime(Booking assumedBookingTime)
        {
            var bookingsOnTheSameDay =  _bookingService.GetBookings(b => b.Date == assumedBookingTime.Date.Date &&
            b.Executor.ChatId == assumedBookingTime.Executor.ChatId);
            //foreach(var booking in bookingsOnTheSameDay)
            //    TimeZoneService.SetProperTimeZone<Booking>(booking, assumedBookingTime.Executor.TimeZoneOffset);
            var isBookingTimeFree = IsBookingTimeFree(assumedBookingTime.BookTimeFrom, assumedBookingTime.BookTimeTo,
                bookingsOnTheSameDay);

            return isBookingTimeFree;
        }

        public async Task<bool> TryToBookTime(Booking initialBooking)
        {
            var timeIsConfirmed = await TryToConfirmBookingTime(initialBooking);

            if (!timeIsConfirmed)
                return false;

            var whenNotify = initialBooking.BookTimeFrom - TimeSpan.FromDays(1);

            //нужно будет доработать
            var messageForClient = $"{initialBooking.User.FirstName}, напоминаем что в {initialBooking.Date.DayOfWeek.ToString()} у Вас забронирован " +
            $"{initialBooking.ServiceAndSpec.Service.Name} у {initialBooking.Executor.FirstName}";

            BackgroundJob.Schedule(() => _botMessageSender.Send(messageForClient, null, initialBooking.User.ChatId), whenNotify);


            initialBooking.Status = BookingCompletionStatus.Confirmed;
            return true;
        }

        /// <summary>
        /// Свободно ли это время? 
        /// </summary>
        /// <param name="BookTimeFrom"></param>
        /// <param name="BookTimeTo"></param>
        /// <param name="bookings"></param>
        /// <returns></returns>
        private bool IsBookingTimeFree(DateTime BookTimeFrom,
            DateTime BookTimeTo, IEnumerable<Booking> bookings)
        {
            foreach (var booking in bookings)
            {
                var o1 = (BookTimeFrom > booking.BookTimeFrom && BookTimeFrom < booking.BookTimeTo);
                var o2 = ((BookTimeTo) > booking.BookTimeFrom);
                var o3 = ((BookTimeTo) < booking.BookTimeTo);
                var o4 = (booking.BookTimeFrom > BookTimeFrom);
                var o5 = (booking.BookTimeFrom < (BookTimeTo));
                var o6 = (booking.BookTimeTo > BookTimeFrom);
                var o7 = (booking.BookTimeTo < (BookTimeTo));

                bool IsbookingTimeFree = (o1 || (o2 && o3)) || ((o4 && o5) || (o6 && o7));

                if (IsbookingTimeFree)
                    return false;
            }
            return true;
        }
    }
}
