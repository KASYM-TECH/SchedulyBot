using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch0
{
    public class S0_SelectIncomingBookingStep : IDialogStep
    {
        private string _messageTag = "branch0step0mess";
        private string _errorMessage = TextTag.BtnErr;

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string>
        {
            {typeof(S1_WhatToDoWithSelectedBookingStep), null }
        };
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly ISelectBookingToConfirmInlineKeyboard _selectBookingToConfirmInlineKeyboard;
        private readonly IUpdateContainer _updateContainer;
        public S0_SelectIncomingBookingStep(ISelectBookingToConfirmInlineKeyboard selectBookingToConfirmInlineKeyboard,
            IUpdateContainer updateContainer,
            IBookingService bookingService,
            IBotMessageManager botMessageSender,
            IMessageTranslationManger messageTranslationManger)
        {
            _bookingService = bookingService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _updateContainer = updateContainer;
            _selectBookingToConfirmInlineKeyboard = selectBookingToConfirmInlineKeyboard;
        }

        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            await _botMessageSender.RemoveButtons(message);
            var allUnconfirmedBookings = _bookingService.GetBookings(b => (b.Status == BookingCompletionStatus.AwaitingConfirmation
            || b.Status == BookingCompletionStatus.IsInConfirmingTemplate)
            && b.Executor.Id == currentDialog.Client.Id);
            if(allUnconfirmedBookings.Count != 0)
                await _selectBookingToConfirmInlineKeyboard.Launch(currentDialog);
            else
            {
                currentDialog.CurrentStepType = null;
                currentDialog.State = CurrentDialogState.Suspended;
            }
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            await _selectBookingToConfirmInlineKeyboard.Handle(currentDialog, _updateContainer.Update);
            //Если был вызван HandleKeyboardExit, и CurrentStepType менять уже не нужно 
            if (currentDialog.State == CurrentDialogState.BeingUsedByTemplate)
                return currentDialog;

            if (currentDialog.CurrentInlineKeyboard == null)
                currentDialog.CurrentStepType = NextStepTextTag.First().Key.ToString();
            return currentDialog;
        }

        public int TemplateId { get; } = 3;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 0;
    }
}
