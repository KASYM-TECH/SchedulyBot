using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch2;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0
{
    public class S8_SelectWeekDaysStep : IDialogStep
    {
        private string _messageTag = "branch0step8mess";
        private string _messageAfterOneOrMoreDaysAreSelectedTag = "branch0step8mess1";
        private string _errorMessage = TextTag.BtnErr;
        private static string _setUpLaterBtnTag = "branch0step8btn0";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IScheduleService _scheduleService;
        private readonly UpdateContainer _container;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;

        private List<WeekDay> _primaryWeekdaysCollection;
        
        public S8_SelectWeekDaysStep(IMessageTranslationManger messageTranslationManger,
            IScheduleService scheduleService, 
            IBotMessageManager botMessageSender, 
            UpdateContainer container,
            BotReplyKeyboardMarkupHandler markupHandler)
        {
            _scheduleService = scheduleService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _container = container;
            _markupHandler = markupHandler;
            
            _primaryWeekdaysCollection = new List<WeekDay>
            {
                WeekDay.Monday, 
                WeekDay.Tuesday, 
                WeekDay.Wednesday, 
                WeekDay.Thursday, 
                WeekDay.Friday, 
                WeekDay.Saturday,
                WeekDay.Sunday
            };
        }
        
        public async Task SendReplyToUser()
        {
            await _scheduleService.DeleteAllByClientId(_container.Client!.Id);
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            var buttons = new List<InlineKeyboardButton>();
            foreach (var weekDay in _primaryWeekdaysCollection)
            {
                var button = InlineKeyboardButton.WithCallbackData(
                    await _messageTranslationManger.GetTextByTag(CollectionName.DaysOfWeek, weekDay.ToString()), weekDay.ToString());
                buttons.Add(button);
            }

            var markup = await _markupHandler.FormInlineKeyboardMarkupFromButtons(buttons, false, false);
            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var callbackQuery = _container.Update.CallbackQuery;
            if (callbackQuery?.Data != null &&
                await _messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, callbackQuery?.Data) == TextTag.DoneBtn)
            {
                await _botMessageSender.DeleteInlineKeyboard(_container.Update!.Message!.MessageId);
                return this.NextStep();
            }
            
            var existSchedules = await _scheduleService.GetManyByClientId(_container.Client!.Id);
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            if (existSchedules.Any())
                message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageAfterOneOrMoreDaysAreSelectedTag);

            var reservedWeekdays = existSchedules.Select(w => w.WeekDay).ToList();
            var buttons = new List<List<InlineKeyboardButton>>();
            foreach (var weekDay in _primaryWeekdaysCollection)
            {
                if (reservedWeekdays.Contains(weekDay))
                    continue;
                var button = InlineKeyboardButton.WithCallbackData(
                    await _messageTranslationManger.GetTextByTag(CollectionName.DaysOfWeek, weekDay.ToString()), weekDay.ToString());
                buttons.Add(new List<InlineKeyboardButton>{button});
            }
            
            if (callbackQuery?.Data == null)
            {
                await _botMessageSender.Send(
                    await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _errorMessage),
                    new InlineKeyboardMarkup(buttons));
                return this.CurrentStep();
            }

            var newSchedule = new Schedule
            {
                ClientId = _container.Client!.Id,
                WeekDay = Enum.Parse<WeekDay>(callbackQuery.Data)
            };

            if (existSchedules.Any(s => s.WeekDay == newSchedule.WeekDay))
                return this.CurrentStepWithoutMessage();
            
            await _scheduleService.Create(newSchedule);
            // Убираем из кнопок выбор юзера
            var buttonsWithoutUserChoice = buttons
                .Where(bl => bl.All(b => b.CallbackData != callbackQuery.Data))
                .ToList();

            var markup = await _markupHandler.FormInlineKeyboardMarkupFromButtons(buttonsWithoutUserChoice, false, false, true);
            await _botMessageSender.EditInlineKeyboard(callbackQuery!.Message!.MessageId, message, markup);
            if (!buttonsWithoutUserChoice.Any())
            {
                await _botMessageSender.DeleteInlineKeyboard(_container.Update!.Message!.MessageId);
                return this.NextStep();
            }
            return this.CurrentStepWithoutMessage();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 8;
    }
}
