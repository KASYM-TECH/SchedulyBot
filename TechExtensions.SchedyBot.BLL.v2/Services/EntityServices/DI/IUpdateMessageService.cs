using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface IUpdateMessageService
    {
        public Task Create(UpdateMessage updateMessage);
        public Task Deactivate(int messageId);
        Task<List<UpdateMessage>?> GetByHashCode(long messageId, string chatId);
        public Task Delete(UpdateMessage updateMessage);
        public Task DeleteAllPreceding(int id, string chatId);
    }
}
