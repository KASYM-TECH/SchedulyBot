using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment
{
    public class BookingManagmentTemplate : IDialogTemplate
    {
        private readonly ILogger<BookingManagmentTemplate> _logger;
        
        public BookingManagmentTemplate(IEnumerable<IDialogStep> dialogSteps, ILogger<BookingManagmentTemplate> logger)
        {
            _logger = logger;
            DialogSteps = dialogSteps.Where(s => s.TemplateId == (int)TemplateId).ToList();
        }
        
        public (int branchId, int stepId)? GetBranchAndStepByState(TemplateStateEnum templateState)
        {
            switch (templateState)
            {
                case TemplateStateEnum.IncomingBookingsStart:
                    return (0, 0);
                case TemplateStateEnum.IncomingBookingManage:
                    return (0, 2);
                case TemplateStateEnum.OutgoingBookingsStart:
                    return (2, 0);
                case TemplateStateEnum.OutgoingBookingManage:
                    return (2, 1);
            }

            _logger.LogCritical($"Не удалось в темплейте найти бранч и степ по enum {templateState.ToString()}");
            return null;
        }
        
        public async Task OnCancel()
        {
        }

        public string TranslationCollectionName { get; } = CollectionName.BookingManagmentTemplate;

        public List<IDialogStep> DialogSteps { get; }
        public TemplateEnum TemplateId { get; } = TemplateEnum.BookingManagmentTemplate;
    }
}
