using TechExtensions.SchedyBot.DLL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.BLL.ReservationManager.Abstractions
{
    public interface IReservationManager
    {
        /// <summary>
        /// Проверяет свободое ли это время
        /// </summary>
        /// <param name="assumedBookingTime"></param>
        /// <returns></returns>
        public Task<bool> TryToConfirmBookingTime(Booking assumedBookingTime);

        /// <summary>
        /// Возвращает true если резервация прошла успешно 
        /// </summary>
        /// <param name="reservationTime"></param>
        /// <param name="restaurant"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public Task<bool> TryToBookTime(Booking initialBooking);

        /// <summary>
        /// Возвращает словарь(время и длительность) свободного времени в заданный день
        /// При условии что day == 00 h 00 min 00 c
        /// </summary>
        /// <param name="restaurant"> Ресторан </param>
        /// <param name="day"> День резервации столика </param>
        /// <returns></returns>
        public Task<Dictionary<DateTime, DateTime>> GetFreeTimeScheduleByDay(Schedule schedule, DateTime bookingDate,
         long executorChatId, TimeSpan assumedDuration, int GapBetweenBookingsInMinutes = 5);
    }
}
