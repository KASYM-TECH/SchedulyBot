using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0
{
    public class S11_DetermineDurationStep : IDialogStep
    {
        private string _messageTag = "branch0step11mess";
        private string _errorMessage = "branch0timeErr";


        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly UpdateContainer _updateContainer;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        
        public S11_DetermineDurationStep(IMessageTranslationManger messageTranslationManger,
            IServiceAndSpecService serviceAndSpecService,
            UpdateContainer updateContainer,
            IBotMessageManager botMessageSender, BotReplyKeyboardMarkupHandler markupHandler)
        {
            _updateContainer = updateContainer;
            _serviceAndSpecService = serviceAndSpecService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _markupHandler = markupHandler;
        }
        
        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(_updateContainer.Template!.TranslationCollectionName, _messageTag);
            var markup = await TimeInputInlineKeyBoard.Launch(_messageTranslationManger, "1:00");
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var handleResult = await TimeInputInlineKeyBoard.Handle(_messageTranslationManger, _updateContainer);

            if (handleResult.result != null)
            {
                var serviceAndSpec = await _serviceAndSpecService.GetByClientId(_updateContainer.Client!.Id);
                serviceAndSpec.Duration = TimeSpan.Parse(handleResult.result);
                await _botMessageSender.DeleteInlineKeyboard(_updateContainer.Update.Message.MessageId);
                return this.NextStep();
            }
            
            if (handleResult.markup != null)
            {
                await _botMessageSender.EditInlineKeyboard(
                    _updateContainer.Update.CallbackQuery!.Message!.MessageId,
                    await _messageTranslationManger.GetTextByTag(_updateContainer.Template!.TranslationCollectionName, _messageTag),
                    handleResult.markup);
                return this.CurrentStepWithoutMessage();
            }

            return null;
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 11;
    }
}
