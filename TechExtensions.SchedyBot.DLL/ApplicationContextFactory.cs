using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TechExtensions.SchedyBot.DLL
{
    public sealed class ApplicationContextFactory : IDesignTimeDbContextFactory<SchedulyBotContext>
    {
        public ApplicationContextFactory()
        {
        }
        public SchedulyBotContext CreateDbContext(string[] args)
        {
            var connection = "Server=localhost;Port=5432;Database=SchedulyBot3;User Id=postgres;Password=kasymov2002;Timeout=300;CommandTimeout=300";
            var options = new DbContextOptionsBuilder<SchedulyBotContext>()
                .UseNpgsql(connection)
                .Options;

            return new SchedulyBotContext(options);
        }
    }
}
