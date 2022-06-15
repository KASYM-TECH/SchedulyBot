using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Containers;

public sealed class UpdateContainer
{
    public long ChatId { get; set; }
    public string? Language { get; set; }
    public Update Update { get; set; }
    public User? User { get; set; }
    public Client? Client { get; set; }
    public bool IsNewClient { get; set; } = false;
    public CurrentDialog? CurrentDialog { get; set; }
    public IDialogTemplate? Template { get; set; }
    public IDialogStep? Step { get; set; }
}