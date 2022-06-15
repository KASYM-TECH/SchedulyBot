using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface IClientService
    {
        public Task Create(Client user);
        public Task Update(Client user);
        public Task<List<Client>?> GetManyUsers(Func<Client, bool> predicate);
        public Task<Client?> GetById(int clientId);
        public Task<Client?> GetUserByChatId(long chatId);
        public Task<Client?> GetByIdUntracked(int clientId);
    }
}
