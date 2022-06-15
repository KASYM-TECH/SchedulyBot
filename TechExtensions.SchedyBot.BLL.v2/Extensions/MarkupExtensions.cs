using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.Extensions;

public static class MarkupExtensions
{
    public static ReplyKeyboardMarkup WithLocationRequest(this ReplyKeyboardMarkup markup)
    {
        markup.Keyboard = markup.Keyboard.Prepend(new List<KeyboardButton>
            { KeyboardButton.WithRequestLocation("Предоставить доступ к локации") });
        return markup;
    }
}