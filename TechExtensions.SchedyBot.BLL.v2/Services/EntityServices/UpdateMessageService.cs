using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    public class UpdateMessageService : IUpdateMessageService
    {
        private readonly IDbRepository<UpdateMessage> _repository;
        private readonly ILogger<UpdateMessageService> _logger;
        public UpdateMessageService(IDbRepository<UpdateMessage> repository, ILogger<UpdateMessageService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Create(UpdateMessage updateMessage)
        {
            try
            {
                await _repository.CreateAsync(updateMessage);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task Deactivate(int messageId)
        {
            var currentMessage = await _repository
                .Get(x => x.MessageId == messageId)
                .FirstOrDefaultAsync();
            if (currentMessage is null)
                return;
            
            currentMessage.IsActive = false;
            await _repository.SaveChangesAsync();
        }

        public async Task<List<UpdateMessage>?> GetByHashCode(long updateId, string chatId)
        {
            try
            {
                var neededHashCode = (updateId, chatId).GetHashCode();
                var updateMessages = _repository.Get(x => x.HashCode == neededHashCode);
                var result = await updateMessages.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Возникла ошибка при поиске моделей update");
                return null;
            }
        }
        
        public async Task DeleteAllPreceding(int id, string chatId)
        {
            {
                try
                {

                    var updateMessages = _repository.Get(u=> u.ChatId == chatId || (u.MessageId < id));
                    var updateMessage = updateMessages.ToList();
                    foreach(var u in updateMessage)
                    {
                        u.CreationDate = DateTime.SpecifyKind(u.CreationDate, kind: DateTimeKind.Utc);
                        _repository.Delete(u);

                    }
                    await _repository.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e.Message);
                }
            }
        }
        public async Task Delete(UpdateMessage updateMessage)
        {
            try
            {
                _repository.Delete(updateMessage);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
    }
}
