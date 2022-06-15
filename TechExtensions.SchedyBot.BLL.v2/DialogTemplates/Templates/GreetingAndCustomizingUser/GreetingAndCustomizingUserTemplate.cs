using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser
{
    public class GreetingAndCustomizingUserTemplate : IDialogTemplate
    {
        private readonly ILogger<GreetingAndCustomizingUserTemplate> _logger;
        private readonly UpdateContainer _container;

        private readonly IScheduleService _scheduleService;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly ICurrentDialogService _dialogService;

        public GreetingAndCustomizingUserTemplate(
            IEnumerable<IDialogStep> dialogSteps, 
            ILogger<GreetingAndCustomizingUserTemplate> logger, 
            UpdateContainer container, IScheduleService scheduleService, IServiceAndSpecService serviceAndSpecService, ICurrentDialogService dialogService)
        {
            _logger = logger;
            _container = container;
            _scheduleService = scheduleService;
            _serviceAndSpecService = serviceAndSpecService;
            _dialogService = dialogService;
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

        public List<IDialogStep> DialogSteps { get; }
        public TemplateEnum TemplateId { get; } = TemplateEnum.GreetingAndCustomizingUserTemplate;

        public string TranslationCollectionName { get; } = CollectionName.GreetingAndCustomizingUserTemplate;
        
        public async Task OnCancel()
        {
            var clientId = _container.Client!.Id;
            var currentServiceAndSpec = await _serviceAndSpecService.GetByClientId(clientId);
            if (currentServiceAndSpec != null)
                await _serviceAndSpecService.Delete(currentServiceAndSpec);
            await _scheduleService.DeleteAllByClientId(clientId);
            _container.Client.IsSeller = false;
        }
    }
}
