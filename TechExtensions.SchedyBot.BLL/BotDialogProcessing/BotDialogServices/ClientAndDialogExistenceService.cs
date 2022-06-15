using TechExtensions.SchedyBot.DLL.Entities;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers.Abstractions;
using TechExtensions.SchedyBot.BLL.Services.Abstractions;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.BotDialogServices
{
    public class ClientAndDialogExistenceService : IClientAndDialogExistenceService
    {
        private readonly IClientService _clientService;
        private readonly ILanguageInfoContainer _languageInfoContainer;
        private readonly ICurrentDialogService _currentDialogService;
        private readonly IChatIDContainer _chatIDContainer;
        public ClientAndDialogExistenceService(IClientService clientService,
            IChatIDContainer chatIDContainer,
            ILanguageInfoContainer languageInfoContainer,
            ICurrentDialogService currentDialogService)
        {
            _languageInfoContainer = languageInfoContainer;
            _chatIDContainer = chatIDContainer;
            _clientService = clientService;
            _currentDialogService = currentDialogService;
        }
        public async Task<UserDao> ReturnExistClientOrJustCreated()
        {
            var foundClient = await _clientService.GetUser(u => u.ChatId == _chatIDContainer.ChatId);
            if (foundClient != null)
                return foundClient;

            var newClient = new UserDao() { ChatId = _chatIDContainer.ChatId, Language = _languageInfoContainer.languageCode };
            await _clientService.Create(newClient);
            return newClient;
        }

        public async Task<CurrentDialog> ReturnExistDialogOrJustCreated(UserDao userDao)
        {
            var clients = _clientService.GetManyUsers(c => c.IsActive);
            var foundCurrentDialog = _currentDialogService.GetCurrentDialog(c => userDao.ChatId  == c.Client.ChatId );
            if (foundCurrentDialog != null)
                return foundCurrentDialog;

            var newCurrentDialog = new CurrentDialog() { Client = userDao };
            await _currentDialogService.CreateCurrentDialog(newCurrentDialog);
            return newCurrentDialog;
        }
    }
}
