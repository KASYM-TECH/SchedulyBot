using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler
{
    public class CustomTemplateManager
    {
        private readonly ICurrentDialogService _currentDialogService;
        private readonly TemplateService _templateService;
        private UpdateContainer _container;
        private readonly ILogger<CustomTemplateManager> _logger;
        public CustomTemplateManager(ICurrentDialogService currentDialogService,
            TemplateService templateService,
            UpdateContainer container, 
            ILogger<CustomTemplateManager> logger)
        {
            _templateService = templateService;
            _currentDialogService = currentDialogService;
            _container = container;
            _logger = logger;
        }
        public static List<Step>? GetByEnum(CustomTemplateEnum customTemplateEnum)
        {
            switch (customTemplateEnum)
            {
                case CustomTemplateEnum.EditSchedule:
                    return new List<Step> { new Step(0, 0, 8), new Step(0, 0, 9), new Step(0, 0, 10), new Step(0, 0, 13), new Step(0, 0, 14) };
                case CustomTemplateEnum.EditProfile:
                    return new List<Step> { new Step(0, 0, 1), new Step(0, 0, 2), new Step(0, 0, 3) };
                case CustomTemplateEnum.EditService:
                    return new List<Step> { new Step(0, 0, 6), new Step(0, 0, 7), new Step(0, 0, 11)};
                default:
                    return null;
            }
        }

        public async Task CreateByEnum(CustomTemplateEnum customTemplateEnum)
        {

            var steps = GetByEnum(customTemplateEnum);
            if (steps == null)
            {
                _logger.LogCritical("Не нашли нужный темплейт");
                return;
            }
            var firstStep = steps[0];
            var template = _templateService.GetTemplateByEnum((TemplateEnum)firstStep.TemplateId);

            var dialog = new CurrentDialog
            {
                ChatId = _container.ChatId,
                CurrentTemplateId = (int)template!.TemplateId,
                CurrentBranchId = firstStep.BranchId,
                CurrentStepId = firstStep.StepId,
                State = CurrentDialogState.Started,
                StepRoute = steps
            };

            await _currentDialogService.CreateCurrentDialog(dialog);
            _container.CurrentDialog = dialog;
            _container.Template = template;
        }
    }

}
