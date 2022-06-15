using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections;

public static class CalendarWithOptionsKeyboard
{
    public static InlineKeyboardMarkup Launch(string selectedDate, List<InlineKeyboardButton> options, bool twoColumns = true)
    {
        var date = DateTime.Parse(selectedDate, CultureInfo.InvariantCulture);
        var todayIsNotSelectedDay = date > DateTime.Now;
        var dateButtons = new List<InlineKeyboardButton>
        {
            new InlineKeyboardButton(todayIsNotSelectedDay ? "◀️" : "✖️")
            {
                CallbackData = todayIsNotSelectedDay ?
                    DateOnly.FromDateTime(date.AddDays(-1)).ToString(CultureInfo.InvariantCulture) :
                    "blank"
            },
            new InlineKeyboardButton(date.ToString("dd.MM.yyyy"))
            {
                CallbackData = "blank"
            },
            new InlineKeyboardButton("▶️")
            {
                CallbackData = DateOnly.FromDateTime(date.AddDays(1)).ToString(CultureInfo.InvariantCulture)
            }
        };

        var buttons = new List<List<InlineKeyboardButton>>();
        buttons.Add(dateButtons);

        if (twoColumns)
            for (var i = 0; i < options.Count; i += 2)
            {
                var buttonStack = new List<InlineKeyboardButton>();
                buttonStack.Add(options[i]);
                if (i+2 <= options.Count)
                    buttonStack.Add(options[i+1]);
                
                buttons.Add(buttonStack);
            }
        else
            foreach (var option in options)
            {
                buttons.Add(new List<InlineKeyboardButton>{option});
            }
        
        var markup = new InlineKeyboardMarkup(buttons);
        return markup;
    }

    public static InlineKeyboardMarkup Launch(DateTime selectedDate, List<InlineKeyboardButton> options, bool twoColumns = true)
    {
        var selectedDateString = selectedDate.ToString(CultureInfo.InvariantCulture);
        return Launch(selectedDateString, options, twoColumns);
    }
}