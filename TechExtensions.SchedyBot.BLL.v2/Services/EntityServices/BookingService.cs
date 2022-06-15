using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    public class BookingService : IBookingService
    {
        private readonly IDbRepository<Booking> _repository;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly ILogger<BookingService> _logger;
        public BookingService(
            IDbRepository<Booking> repository,
            IServiceAndSpecService serviceAndSpecService,
            ILogger<BookingService> logger)
        {
            _serviceAndSpecService = serviceAndSpecService;
            _repository = repository;
            _logger = logger;
        }

        public async Task Create(Booking booking)
        {
            try
            {
                booking.Status = BookingCompletionStatus.New;

                var alreadyExists = GetBookings(b => b.Client.ChatId == booking.Client.ChatId && b.Executor.ChatId == booking.Executor.ChatId 
                && b.Status == BookingCompletionStatus.New);
                if (alreadyExists != null && alreadyExists!.Any())
                    return;
                await _repository.CreateAsync(booking);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
        public async Task Delete(Booking booking)
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

        public async Task DeleteAllNewByClientId(int clientId)
        {
            try
            {
                var newBookings = await _repository
                    .Get(x => x.ClientId == clientId)
                    .Where(x => x.Status == BookingCompletionStatus.New)
                    .ToListAsync();
                
                foreach (var newBooking in newBookings)
                    await Delete(newBooking);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task<List<Booking>> GetBookingsByExecutorIdAndDate(int executorId, DateTime date)
        {
            var bookings = await _repository.Get(x => x.Executor.Id == executorId)
                .Where(x => x.Date == date)
                .ToListAsync();

            return bookings;
        }

        public List<Booking> GetBookings(Expression<Func<Booking, bool>> predicate)
        {
            try
            {
                var bookings =  _repository.Get(predicate)
                    .Include(b=> b.Client)
                    .Include(b => b.ServiceAndSpec)
                    .Include(b => b.Executor)?
                    .ToList();
                if(bookings == null)
                    _logger.LogWarning("Объект Bookings не найден");

                return bookings;
            }
            catch(Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }
        
        public async Task<Booking?> GetNewByClientId(int clientId)
        {
            try
            {
                var booking = await _repository.Get(x => x.ClientId == clientId)
                    .Where(x => x.Status == BookingCompletionStatus.New)
                    .Include(b => b.Client)
                    .Include(b => b.Executor)
                    .Include(b => b.ServiceAndSpec)
                    .ThenInclude(s => s.Service)
                    .FirstOrDefaultAsync();
                    
                if (booking == null)
                    _logger.LogWarning("Объект Booking не найден");

                return booking; 
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<Booking?> GetById(int bookingId)
        {
            try
            {
                var booking = await _repository.Get(x => x.Id == bookingId)
                    .Include(b => b.Client)
                    .Include(b => b.Executor)
                    .Include(b => b.ServiceAndSpec)
                    .ThenInclude(s => s.Service)
                    .FirstOrDefaultAsync();
                    
                if (booking == null)
                    _logger.LogWarning("Объект Booking не найден");

                return booking; 
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<List<Booking>> GetAllByClientId(int clientId)
        {
            try
            {
                var booking = await _repository.Get(x => x.ClientId == clientId)
                    .Include(b => b.Client)
                    .Include(b => b.Executor)
                    .Include(b => b.ServiceAndSpec)
                    .ThenInclude(s => s.Service)
                    .ToListAsync();

                return booking; 
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return new List<Booking>();
            }
        }

        public async Task<List<Booking>> GetAllByClientIdAndDate(int clientId, DateTime date)
        {
            try
            {
                var booking = await _repository.Get(x => x.ClientId == clientId)
                    .Where(x => x.Date == date)
                    .Include(b => b.Client)
                    .Include(b => b.Executor)
                    .Include(b => b.ServiceAndSpec)
                    .ThenInclude(s => s.Service)
                    .ToListAsync();

                return booking; 
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return new List<Booking>();
            }
        }

        public async Task<List<Booking>> GetAllByExecutorId(int executorId)
        {
            try
            {
                var booking = await _repository.Get(x => x.ExecutorId == executorId)
                    .Include(b => b.Client)
                    .Include(b => b.Executor)
                    .Include(b => b.ServiceAndSpec)
                    .ThenInclude(s => s.Service)
                    .ToListAsync();

                return booking; 
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return new List<Booking>();
            }
        }

        public async Task<List<Booking>> GetAllByExecutorIdAndDate(int executorId, DateTime date)
        {
            try
            {
                var booking = await _repository.Get(x => x.ExecutorId == executorId)
                    .Where(x => x.Date == date)
                    .Include(b => b.Client)
                    .Include(b => b.Executor)
                    .Include(b => b.ServiceAndSpec)
                    .ThenInclude(s => s.Service)
                    .ToListAsync();

                return booking; 
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return new List<Booking>();
            }
        }

        public async Task Update(Booking booking)
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
