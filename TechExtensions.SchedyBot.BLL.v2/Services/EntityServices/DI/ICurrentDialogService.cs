using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface ICurrentDialogService
    {
        public Task CreateCurrentDialog(CurrentDialog currentDialog);
        Task<CurrentDialog?> GetByChatId(long chatId);
        Task<List<CurrentDialog>?> GetManyByChatId(long chatId);
        public Task Update(CurrentDialog currentDialog);
        public Task Delete(CurrentDialog currentDialog);
        public Task DeleteAllByChatId(long chatId);
    }
}
