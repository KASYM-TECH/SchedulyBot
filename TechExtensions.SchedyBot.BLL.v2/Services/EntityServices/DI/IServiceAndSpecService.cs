using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface IServiceAndSpecService
    {
        public Task Create(ServiceAndSpec service);
        public ServiceAndSpec GetServiceAndSpec(Func<ServiceAndSpec, bool> predicate);
        /// <summary>
        /// Достает последнюю по Id сущность по Id клиента
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public Task<ServiceAndSpec?> GetByClientId(int clientId);
        public Task<ServiceAndSpec?> GetById(int sasId);
        public Task Update(ServiceAndSpec user);
        Task<List<ServiceAndSpec>> GetManyByClientId(int clientId);
        public Task<ServiceAndSpec?> GetByClientIdUntracked(int clientId);
        public Task Delete(ServiceAndSpec serviceAndSpec);
    }
}
