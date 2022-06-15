using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface IBookingInConfirmationService
    {
        public Task Create(BookingInConfirmation bookingInConfirmation);
        public Task Delete(BookingInConfirmation booking);
        public Task DeleteMany(IEnumerable<BookingInConfirmation> bookings);
        public List<BookingInConfirmation>? GetMany(Func<BookingInConfirmation, bool> predicate);
        public BookingInConfirmation? Get(Func<BookingInConfirmation, bool> predicate);
        public Task Update(BookingInConfirmation booking);

    }
}
