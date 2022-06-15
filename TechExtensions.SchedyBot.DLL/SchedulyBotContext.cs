using Microsoft.EntityFrameworkCore;
using System.Data.Entity.ModelConfiguration.Conventions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL
{
    public class SchedulyBotContext : DbContext
    {
        public DbSet<ActionHistory> ActionsHistory { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<CurrentDialog> CurrentDialogs { get; set; }
        public DbSet<UpdateMessage> UpdateMessages { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceAndSpec> ServiceAndSpecs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<BookingInConfirmation> BookingInConfirmations { get; set; }
        public DbSet<CurrentDialogIteration> CurrentDialogIterations { get; set; }
        public DbSet<Step> Steps { get; set; }

        public SchedulyBotContext(DbContextOptions<SchedulyBotContext> options) : base(options) { }
    }
}
