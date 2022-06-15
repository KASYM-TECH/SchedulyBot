using System.Linq.Expressions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface IBookingService
    {
        public List<Booking> GetBookings(Expression<Func<Booking, bool>> predicate);
        public Task<Booking?> GetNewByClientId(int clientId);
        public Task<Booking?> GetById(int bookingId);
        public Task<List<Booking>> GetAllByClientId(int clientId);
        public Task<List<Booking>> GetAllByClientIdAndDate(int clientId, DateTime date);
        public Task<List<Booking>> GetAllByExecutorId(int executorId);
        public Task<List<Booking>> GetAllByExecutorIdAndDate(int executorId, DateTime date);
        public Task Update(Booking user);
        public Task Create(Booking booking);
        public Task Delete(Booking booking);

        Task DeleteAllNewByClientId(int clientId);
        Task<List<Booking>> GetBookingsByExecutorIdAndDate(int executorId, DateTime date);
    }
}
