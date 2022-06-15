using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch0
{
    public class S1_ChooseServiceStep : IDialogStep
    {
        private string _messageTag = "branch0step1mess";
        private string _errorMessage = TextTag.BtnErr;

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly UpdateContainer _updateContainer;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        
        public S1_ChooseServiceStep(IMessageTranslationManger messageTranslationManger,
           IBotMessageManager botMessageSender,
           UpdateContainer updateContainer,
           IBookingService bookingService,
           IServiceAndSpecService serviceAndSpecService,
           BotReplyKeyboardMarkupHandler markupHandler)
        {
            _markupHandler = markupHandler;
            _serviceAndSpecService = serviceAndSpecService;
            _bookingService = bookingService;
            _updateContainer = updateContainer;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }

        public async Task SendReplyToUser()
        {
            var booking = await _bookingService.GetNewByClientId(_updateContainer.Client!.Id);
            if (booking == null)
                throw new NullReferenceException(
                    "В предыдущем шаге мы создали запись в таблицу bookings, теперь не можем её найти");

            var buttons = await GetButtonsFromServices(booking.ExecutorId.Value);
            var message = await _messageTranslationManger.GetTextByTag(_updateContainer.Template!.TranslationCollectionName, _messageTag);
            var markup = await _markupHandler.FormInlineKeyboardMarkupFromButtons(buttons);
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var booking = await _bookingService.GetNewByClientId(_updateContainer.Client!.Id);
            if (booking == null)
                throw new NullReferenceException(
                    "В предыдущем шаге мы создали запись в таблицу bookings, теперь не можем её найти");

            var callbackQueryData = _updateContainer.Update.CallbackQuery?.Data;
            if (callbackQueryData == null)
            {
                await _botMessageSender.DeleteInlineKeyboard(_updateContainer.Update.Message.MessageId);
                var message = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _errorMessage);
                await _botMessageSender.Send(message);

                return this.CurrentStepWithoutMessage();
            }

            var serviceAndSpecId = int.Parse(callbackQueryData);
            booking.ServiceAndSpecId = serviceAndSpecId;
            await _bookingService.Update(booking);
            await _botMessageSender.DeleteInlineKeyboard(_updateContainer.Update.Message.MessageId);
            
            return this.NextStep();
        }

        private async Task<List<InlineKeyboardButton>> GetButtonsFromServices(int bookingExecutorId)
        {
            // TODO: Обработать ситуацию, когда у исполнителя нет услуг
            var servicesAndSpecs = await _serviceAndSpecService.GetManyByClientId(bookingExecutorId);
            var buttons = new List<InlineKeyboardButton>();
            foreach (var servicesAndSpec in servicesAndSpecs)
            {
                var button = InlineKeyboardButton.WithCallbackData(servicesAndSpec.Service.Name,
                    servicesAndSpec.Id.ToString());
                buttons.Add(button);
            }

            return buttons;
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int) TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 1;
    }
}
