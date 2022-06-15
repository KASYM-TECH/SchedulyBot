using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;


namespace TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler
{
    public class MainMenuManager
    {
        private UpdateContainer _container;
        private readonly TemplateService _templateService;
        private readonly IBotMessageManager _messageManager;
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly ICurrentDialogService _currentDialogService;
        private readonly CustomTemplateManager _customTemplateManager;

        public MainMenuManager(
            UpdateContainer container,
            ICurrentDialogService currentDialogService,
            CustomTemplateManager customTemplateManager,
            IMessageTranslationManger messageTranslationManger,
            TemplateService templateService,
            IBotMessageManager messageManager, BotReplyKeyboardMarkupHandler markupHandler)
        {
            _container = container;
            _currentDialogService = currentDialogService;
            _customTemplateManager = customTemplateManager;
            _messageTranslationManger = messageTranslationManger;
            _templateService = templateService;
            _messageManager = messageManager;
        }

        public async Task Manage(string text)
        {
            var mainMenuBtnTranslationTag = await _messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, text);
            if (mainMenuBtnTranslationTag == null)
                return;

            var wentThroughFullRegistration = _container.Client!.WentThroughFullRegistration;
            //Не прошел регистрацию
            if (wentThroughFullRegistration is false && (mainMenuBtnTranslationTag == MainMenuAction.UpdateSchedule || 
                mainMenuBtnTranslationTag == MainMenuAction.UpdateProfile ||
                mainMenuBtnTranslationTag == MainMenuAction.EditService))
            {
                await _messageManager.ReturnMainButtonsToClient();
                return;
            }

            switch (mainMenuBtnTranslationTag)
            {
                case MainMenuAction.UpdateSchedule:
                    await _customTemplateManager.CreateByEnum(CustomTemplateEnum.EditSchedule);
                    break;
                case MainMenuAction.UpdateProfile:
                    await _customTemplateManager.CreateByEnum(CustomTemplateEnum.EditProfile);
                    break;
                case MainMenuAction.EditService:
                    await _customTemplateManager.CreateByEnum(CustomTemplateEnum.EditService);
                    break;
                case MainMenuAction.GoThroughFullRegistration:
                    await LaunchRegistrationTemplate();
                    break;
            }
        }
        private async Task LaunchRegistrationTemplate()
        {
            var template = _templateService.GetTemplateByEnum(TemplateEnum.GreetingAndCustomizingUserTemplate);
            _container.Template = template;

            // Теперь создадим текущий диалог
            var dialog = new CurrentDialog
            {
                ChatId = _container.ChatId,
                CurrentTemplateId = 0,
                CurrentBranchId = 0,
                CurrentStepId = 0,
                State = CurrentDialogState.Started
            };

            await _currentDialogService.CreateCurrentDialog(dialog);
            _container.CurrentDialog = dialog;
        }
    }
}
