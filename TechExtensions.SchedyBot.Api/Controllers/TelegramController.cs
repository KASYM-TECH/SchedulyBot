using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.TelegramBotBasics;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.Api.Controllers
{
    public class TelegramController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService, [FromBody] Update update)
        {
            await handleUpdateService.EchoAsync(update);
            return Ok();
        }
    }
}
