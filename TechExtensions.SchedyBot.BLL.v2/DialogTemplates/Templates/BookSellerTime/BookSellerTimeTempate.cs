using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime
{
    public class BookSellerTimeTemplate : IDialogTemplate
    {
        private readonly ILogger<BookSellerTimeTemplate> _logger;
        private readonly UpdateContainer _container;
        private readonly ICurrentDialogService _dialogService;
        private readonly IBookingService _bookingService;
        
        public BookSellerTimeTemplate(IEnumerable<IDialogStep> dialogSteps, ILogger<BookSellerTimeTemplate> logger, UpdateContainer container, ICurrentDialogService dialogService, IBookingService bookingService)
        {
            _logger = logger;
            _container = container;
            _dialogService = dialogService;
            _bookingService = bookingService;
            DialogSteps = dialogSteps.Where(s => s.TemplateId == (int)TemplateId).ToList();
        }
        
        public (int branchId, int stepId)? GetBranchAndStepByState(TemplateStateEnum templateState)
        {
            switch (templateState)
            {
                // TODO: Объединить степы шаблона с регистрацией
                case TemplateStateEnum.Start:
                    return (1, 0);
                case TemplateStateEnum.ClientExists:
                    return (0, 0);
            }

            _logger.LogCritical($"Не удалось в темплейте найти бранч и степ по enum {templateState.ToString()}");
            return null;
        }

        public async Task OnCancel()
        {
            await _bookingService.DeleteAllNewByClientId(_container.Client.Id);
        }

        public List<IDialogStep> DialogSteps { get; }
        public TemplateEnum TemplateId { get; } = TemplateEnum.BookSellerTimeTemplate;
        public string TranslationCollectionName { get; } = CollectionName.BookSellerTimeTemplate;


    }
}

