using System.Collections;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface IServiceService
    {
        public List<Service> GetServices(Func<Service, bool> predicate);
        public Task CreateService(Service service);
        public Service GetService(Func<Service, bool> predicate);
        public Task<Service?> GetByName(string serviceName);
        public Task Update(Service user);
        Task<List<Service>> GetMainServices(string languageCode);
    }
}
