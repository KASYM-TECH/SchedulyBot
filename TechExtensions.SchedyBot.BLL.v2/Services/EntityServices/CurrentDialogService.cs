using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    public class CurrentDialogService : ICurrentDialogService
    {
        private readonly IDbRepository<CurrentDialog> _dialogRepository;
        private readonly ILogger<CurrentDialogService> _logger;

        public CurrentDialogService(
            IDbRepository<CurrentDialog> dialogRepository,
            ILogger<CurrentDialogService> logger)
        {
            _dialogRepository = dialogRepository;
            _logger = logger;
        }

        public async Task CreateCurrentDialog(CurrentDialog currentDialog)
        {
            try
            {
                await DeleteAllByChatId(currentDialog.ChatId);
                await _dialogRepository.CreateAsync(currentDialog);
                await _dialogRepository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task<CurrentDialog?> GetByChatId(long chatId)
        {
            try
            {
                var currentDialog = await _dialogRepository
                    .Get(x => x.ChatId == chatId)
                    .Include(s => s.CurrentDialogIterations)
                    .Include(c=> c.StepRoute)
                    .FirstOrDefaultAsync();

                if (currentDialog == null)
                    _logger.LogWarning("Объект CurrentDialog не найден");

                return currentDialog;
            }
            catch (Exception e)
            {
                _logger.LogCritical("shit", e);
                return null;
            }
        }

        public async Task<List<CurrentDialog>?> GetManyByChatId(long chatId)
        {
            try
            {
                var currentDialog = await _dialogRepository
                    .Get(x => x.ChatId == chatId)
                    .Include(s => s.CurrentDialogIterations)
                    .ToListAsync();

                return currentDialog;
            }
            catch (Exception e)
            {
                _logger.LogCritical("shit", e);
                return null;
            }
        }

        public async Task Update(CurrentDialog currentDialog)
        {
            try
            {
                _dialogRepository.Update(currentDialog);
                await _dialogRepository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task Delete(CurrentDialog currentDialog)
        {
            try
            {
                _dialogRepository.Delete(currentDialog);
                await _dialogRepository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Не удалось удалить объект CurrentDialog");
            }
        }

        public async Task DeleteAllByChatId(long chatId)
        {
            var dialogs = await _dialogRepository
                .Get(x => x.ChatId == chatId)
                .ToListAsync();
            
            foreach (var dialog in dialogs)
            {
                await Delete(dialog);
            }
        }
    }
}
