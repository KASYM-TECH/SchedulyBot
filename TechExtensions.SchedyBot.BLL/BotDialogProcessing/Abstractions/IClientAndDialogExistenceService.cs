using TechExtensions.SchedyBot.DLL.Entities;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions
{
    public interface IClientAndDialogExistenceService
    {
        public Task<UserDao> ReturnExistClientOrJustCreated();
        public Task<CurrentDialog> ReturnExistDialogOrJustCreated(UserDao userDao);

    }
}
