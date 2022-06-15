using Hangfire;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;

namespace TechExtensions.SchedyBot.BLL.v2.Services
{
    public class HangFireService
    {
        private readonly IBotMessageManager _botMessageManager;
        private readonly UpdateContainer _updateContainer;
        public HangFireService(IBotMessageManager botMessageManager, UpdateContainer updateContainer)
        {
            _updateContainer = updateContainer;
            _botMessageManager = botMessageManager;
        }

        public string ScheduleTime(DateTime time, TimeSpan timeZoneOffset, long chatId, string hangFireMessage, int offsetInminutes = 60)
        {
            //не уверен будут ли случаи где timeZoneOffset придется вычитать 
            var now = DateTime.UtcNow + timeZoneOffset;
            if (time.AddMinutes(-offsetInminutes) < now)
                return null;
            var timeSpan = time.AddMinutes(-offsetInminutes) - now;
            var jobId = BackgroundJob.Schedule(() => _botMessageManager.Send(hangFireMessage, null, chatId), timeSpan);
            return jobId;
        }

        public void DeleteScheduledTime(string jobId)
        {
            if (jobId == null)
                return;
            BackgroundJob.Delete(jobId);
        }
    }
}
