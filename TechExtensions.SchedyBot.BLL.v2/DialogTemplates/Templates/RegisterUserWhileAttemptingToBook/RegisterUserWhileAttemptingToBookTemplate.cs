using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.RegisterUserWhileAttemptingToBook
{
    public class RegisterUserWhileAttemptingToBookTemplate : IDialogTemplate 
    {
        private readonly ILogger<RegisterUserWhileAttemptingToBookTemplate> _logger;
        
        public RegisterUserWhileAttemptingToBookTemplate(IEnumerable<IDialogStep> dialogSteps, ILogger<RegisterUserWhileAttemptingToBookTemplate> logger)
        {
            _logger = logger;
            DialogSteps = dialogSteps.Where(s => s.TemplateId == (int)TemplateId).ToList();
        }

        public (int branchId, int stepId)? GetBranchAndStepByState(TemplateStateEnum templateState)
        {
            switch (templateState)
            {
                case TemplateStateEnum.Start:
                    return (0, 0);
            }

            _logger.LogCritical($"Не удалось в темплейте найти бранч и степ по enum {templateState.ToString()}");
            return null;
        }

        public string TranslationCollectionName { get; } = CollectionName.RegisterUserWhileAttemptingToBookTemplate;
        public List<IDialogStep> DialogSteps { get; }
        public TemplateEnum TemplateId { get; } = TemplateEnum.RegisterUserWhileAttemptingToBookTemplate;

    }
}
