using System.Data.Entity;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    public class BookingInConfirmationService : IBookingInConfirmationService
    {
        private readonly IDbRepository<BookingInConfirmation> _repository;
        private readonly ILogger<BookingInConfirmationService> _logger;
        private readonly IBookingService _bookingService;
        public BookingInConfirmationService(IDbRepository<BookingInConfirmation> repository,
           IBookingService bookingService,
             ILogger<BookingInConfirmationService> logger)
        {
            _bookingService = bookingService;
            _repository = repository;
            _logger = logger;
        }

        public async Task Create(BookingInConfirmation bookingInConfirmation)
        {
            try
            {
                var outdatedBookings = GetMany(b => b.BookingToConfirm.Executor == bookingInConfirmation.BookingToConfirm.Executor);
                if (outdatedBookings?.Count() != 0)
                    await DeleteMany(outdatedBookings);

                await _repository.CreateAsync(bookingInConfirmation);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
        public async Task Delete(BookingInConfirmation booking)
        {
            try
            {
                _repository.Delete(booking);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task DeleteMany(IEnumerable<BookingInConfirmation> bookings)
        {
            try
            {
                foreach (var b in bookings)
                    await Delete(b);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
        public List<BookingInConfirmation>? GetMany(Func<BookingInConfirmation, bool> predicate)
        {
            try
            {
                var bookings = _repository.Get(null)
                    .Include(b => b.BookingToConfirm)
                    .ToList();
                if (bookings == null)
                    _logger.LogWarning("Объект BookingInConfirmations не найден");

                return bookings;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }
        public BookingInConfirmation? Get(Func<BookingInConfirmation, bool> predicate)
        {
            try
            {
                var bookings = _bookingService.GetBookings(b => b.IsActive);
                var bookingsToConfirm = _repository.Get(null)
                    .Include(b => b.BookingToConfirm);

                if (bookingsToConfirm == null)
                    _logger.LogWarning("Объект BookingInConfirmation не найден");

                return bookingsToConfirm?.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }
        public async Task Update(BookingInConfirmation booking)
        {
            try
            {
                _repository.Update(booking);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
    }
}
