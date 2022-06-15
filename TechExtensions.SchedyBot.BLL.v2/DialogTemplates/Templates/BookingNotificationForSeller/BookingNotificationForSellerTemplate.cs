using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller
{
    public class BookingNotificationForSellerTemplate : IDialogTemplate
    {
        private readonly ILogger<BookingNotificationForSellerTemplate> _logger;
        
        public BookingNotificationForSellerTemplate(IEnumerable<IDialogStep> dialogSteps, ILogger<BookingNotificationForSellerTemplate> logger)
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

        public string TranslationCollectionName { get; } = CollectionName.BookingNotificationForSellerTemplate;
        public List<IDialogStep> DialogSteps { get; }
        public TemplateEnum TemplateId { get; } = TemplateEnum.BookingNotificationForSellerTemplate;
    }
}
