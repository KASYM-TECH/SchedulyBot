using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.Shared.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0
{
    public class S6_SelectServiceStep : IDialogStep
    {
        private string _messageTag = "branch0step6mess";
        private string _errorMessage = "branch0step6err";

        private readonly IMessageTranslationManger _messageTranslationManger;
        // private readonly IServiceAndIndustryRelationManager _serviceAndIndustryRelationManager;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly IServiceService _serviceService;
        private readonly UpdateContainer _updateContainer;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        
        public S6_SelectServiceStep(IMessageTranslationManger messageTranslationManger,
            // IServiceAndIndustryRelationManager serviceAndIndustryRelationManager, 
            IBotMessageManager botMessageSender,
           IServiceAndSpecService serviceAndSpecService,
           UpdateContainer updateContainer, 
            IServiceService serviceService, 
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _serviceAndSpecService = serviceAndSpecService;
            _serviceService = serviceService;
            _markupHandler = markupHandler;
            _updateContainer = updateContainer;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }
        
        public async Task SendReplyToUser()
        {
            var services = await _serviceService.GetMainServices(_updateContainer.Language!);
            var inlineKeyboardButtons = new List<InlineKeyboardButton>();
            foreach (var service in services)
            {
                if (service.Name is not null)
                    inlineKeyboardButtons.Add(InlineKeyboardButton.WithCallbackData(service.Name));
            }
            
            var inlineMarkup = await _markupHandler.FormInlineKeyboardMarkupFromButtons(inlineKeyboardButtons);
            var message = await _messageTranslationManger.GetTextByTag(_updateContainer.Template!.TranslationCollectionName, _messageTag);
            await _botMessageSender.RemoveButtons(message);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            if (clientAnswer.IsNullOrEmpty())
            {
                var message = await _messageTranslationManger.GetTextByTag(_updateContainer.Template!.TranslationCollectionName, _errorMessage);
                await _botMessageSender.Send(message);
                return this.CurrentStep();
            }

            var service = await _serviceService.GetByName(clientAnswer);
            if (service == null)
            {
                service = new Service
                {
                    Name = clientAnswer,
                    LanguageCode = _updateContainer.Client!.Language
                };
                await _serviceService.CreateService(service);
            }

            var serviceAndSpec = await _serviceAndSpecService.GetByClientId(_updateContainer.Client!.Id);
            serviceAndSpec = new ServiceAndSpec
            {
                User = _updateContainer.Client,
                Service = service
            };
            await _serviceAndSpecService.Create(serviceAndSpec);

            serviceAndSpec.Service = service;
            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 6;
    }
}
