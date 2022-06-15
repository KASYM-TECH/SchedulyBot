using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics
{
    public class BotReplyKeyboardMarkupHandler
    {
        private readonly IMessageTranslationManger _translationManger;
        private readonly UpdateContainer _updateContainer;

        public BotReplyKeyboardMarkupHandler(IMessageTranslationManger translationManger, UpdateContainer updateContainer)
        {
            _translationManger = translationManger;
            _updateContainer = updateContainer;
        }
        
        /// <summary>
        /// Формирует ReplyKeyboardMarkup из листа с кнопками 
        /// </summary>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public async Task<ReplyKeyboardMarkup> FormReplyKeyboardMarkupFromButtons(
            List<string>? buttons = null, 
            bool withBackButton = false, 
            bool withCancelButton = false,
            bool withDoneButton = false)
        {
            if(_updateContainer?.CurrentDialog != null)
            {
                if (_updateContainer.CurrentDialog!.StepRoute!.Any())
                {
                    var firstStepInRoute = _updateContainer.CurrentDialog.StepRoute[0];
                    var currntD = _updateContainer.CurrentDialog;
                    //Если мы находимся на первом степе в StepRoute
                    if (firstStepInRoute.TemplateId == currntD.CurrentTemplateId && firstStepInRoute.BranchId == currntD.CurrentBranchId && firstStepInRoute.StepId == currntD.CurrentStepId)
                        withBackButton = false;
                }
            }

            var keyboardButtons = new List<List<KeyboardButton>>();
            // Если null, то отправляем только назад и отмену, например
            if (buttons != null)
                foreach (var button in buttons)
                {
                    var newKeyboardButton = new List<KeyboardButton> {(new KeyboardButton(button))};
                    keyboardButtons.Add(newKeyboardButton);
                }

            if (withBackButton)
                keyboardButtons.Add(
                    new List<KeyboardButton>
                    {
                        new KeyboardButton(
                            await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn)
                            )
                    });

            if (withCancelButton)
                keyboardButtons.Add(
                    new List<KeyboardButton>
                    {
                        new KeyboardButton(
                            await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.CancelBtn)
                            )
                    });

            if (withDoneButton)
                keyboardButtons.Add(
                    new List<KeyboardButton>
                    {
                        new KeyboardButton(
                            await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.DoneBtn)
                            )
                    });
            
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons) { ResizeKeyboard = true };

            return replyKeyboardMarkup;
        }
        
        public async Task<InlineKeyboardMarkup> FormInlineKeyboardMarkupFromButtons(List<List<InlineKeyboardButton>>? buttons = null,
            bool withBackButton = false, bool withCancelButton = false, bool withDoneButton = false)
        {
            var inlineButtons = new List<List<InlineKeyboardButton>>();

            if (withDoneButton)
                inlineButtons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.DoneBtn))
                });
            
            if (buttons != null)
                inlineButtons.AddRange(buttons);

            if (withBackButton)
                inlineButtons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn))
                });

            if (withCancelButton)
                inlineButtons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.CancelBtn))
                });
            
            var inlineMarkup = new InlineKeyboardMarkup(inlineButtons);
            return inlineMarkup;
        }
        
        public async Task<InlineKeyboardMarkup> FormInlineKeyboardMarkupFromButtons(List<InlineKeyboardButton>? buttons = null,
            bool withBackButton = false, bool withCancelButton = false, bool withDoneButton = false)
        {
            var inlineButtons = new List<List<InlineKeyboardButton>>();

            if (withDoneButton)
                inlineButtons.Add(new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(
                            await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.DoneBtn))
                    });
            
            if (buttons != null)
                inlineButtons.AddRange(buttons.Select(b => new List<InlineKeyboardButton>{b}));

            if (withBackButton)
                inlineButtons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn))
                });

            if (withCancelButton)
                inlineButtons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        await _translationManger.GetTextByTag(CollectionName.CommonTranslations, TextTag.CancelBtn))
                });
            
            var inlineMarkup = new InlineKeyboardMarkup(inlineButtons);
            return inlineMarkup;
        }
    }
}
