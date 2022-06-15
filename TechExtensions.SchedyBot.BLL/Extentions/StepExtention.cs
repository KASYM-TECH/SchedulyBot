using System.Linq;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions;

namespace TechExtensions.SchedyBot.BLL.Extentions
{
    public static class StepExtention
    {
        public static string GetStepClassName(this IDialogStep step)
        {
            var stepName = step.GetType().ToString().Split(".").TakeLast(4).ToArray();
            var name = stepName.ElementAt(0) + "." + stepName.ElementAt(1) + "." + stepName.ElementAt(2) + "." + stepName.ElementAt(3);
            return name;
        }
    }
}
