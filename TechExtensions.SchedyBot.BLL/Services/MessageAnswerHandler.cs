using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.Containers;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.Services;

public sealed class MessageAnswerHandler
{
    private UpdateDataContainer _dataContainer;
    
    public MessageAnswerHandler(UpdateDataContainer dataContainer)
    {
        _dataContainer = dataContainer;
    }
    
    public async Task Handle(Message message)
    {
        
    }
}