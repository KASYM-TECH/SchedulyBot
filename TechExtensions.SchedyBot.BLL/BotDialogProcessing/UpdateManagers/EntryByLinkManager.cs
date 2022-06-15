using TechExtensions.SchedyBot.BLL.TelegramBotLinkModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions;
using TechExtensions.SchedyBot.BLL.Models;
using TechExtensions.SchedyBot.BLL.Models.Constants;
using TechExtensions.SchedyBot.BLL.Services.Abstractions;
using TechExtensions.SchedyBot.BLL.TelegramBotLinkModule.Abstractions;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.UpdateManagers
{
    public class EntryByLinkManager : IUpdateManager
    {
        private readonly ITelegramBotLinkManager _telegramBotLinkManager;
        private readonly ICurrentDialogService _currentDialogServcie;
        private readonly IBookingService _bookingService;
        private readonly IClientAndDialogExistenceService _clientAndDialogExistenceService;
        private readonly IClientService _clientService;
        public EntryByLinkManager(ITelegramBotLinkManager telegramBotLinkManager,
            IClientAndDialogExistenceService clientAndDialogExistenceService,
            IBookingService bookingService,
            IClientService clientService,
            ICurrentDialogService currentDialogServcie) 
        {
            _clientService = clientService;
            _bookingService = bookingService;
            _currentDialogServcie = currentDialogServcie;
            _clientAndDialogExistenceService = clientAndDialogExistenceService;
            _telegramBotLinkManager = telegramBotLinkManager;
        }
        public Task<bool> DoesItForMe(UpdateModelForManage model)
        {
            if (model.Update.Message == null)
                return Task.FromResult(false);
            return Task.FromResult(_telegramBotLinkManager.KeyExists(model.Update?.Message?.Text));
        }

        public async Task<bool> Manage(UpdateModelForManage model)
        {
            var chatId = _telegramBotLinkManager.GetChatIdByLink(model.Update?.Message?.Text);
            var executor = await _clientService.GetUser(c => c.ChatId == chatId);
            var foundCurrentDialog = _currentDialogServcie.GetCurrentDialog(c => c.Client.ChatId == model.Update.Message.Chat.Id);
            await _bookingService.CreateBooking(new Booking { User = model.Client, Executor = executor });
            if (foundCurrentDialog!.Client.FirstName == null)
                model.Update.Message.Text = TelegramCommand.RegisterUserWhileAttemptingToBookTemplate;
            else
                model.Update.Message.Text = TelegramCommand.BookSellerTimeTemplate;
            return true;
        }
    }
}
