using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections
{
    public static class TimeInputInlineKeyBoard
    {
        public static async Task<IReplyMarkup> Launch(IMessageTranslationManger messageTranslationManger, string startTimeString)
        {
            var minText = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.Minutes);
            var hourText = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.Hour);
            var doneBtn = await messageTranslationManger
               .GetTextByTag(CollectionName.CommonTranslations, TextTag.DoneBtn);
            
            var hours = int.Parse(startTimeString.Split(":")[0]);
            var mins = int.Parse(startTimeString.Split(":")[1]);
            var buttons = new List<List<InlineKeyboardButton>>() {
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton("🔺 5 " + minText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours, mins + 5)
                    },
                    new InlineKeyboardButton("🔺 10 " + minText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours, mins + 10)
                    },
                    new InlineKeyboardButton("🔺 1 " + hourText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours + 1, 0)
                    }
                },
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton(string.Join(" ", startTimeString, doneBtn))
                    {
                        CallbackData = doneBtn
                    },
                },
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton("🔻 5 " + minText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours, mins - 5)
                    },
                    new InlineKeyboardButton("🔻 10 " + minText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours, mins - 10)
                    },
                    new InlineKeyboardButton("🔻 1 " + hourText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours - 1, 0)
                    }
                }
            };
            
            var markup = new InlineKeyboardMarkup(buttons);

            return markup;
        }

        public static async Task<(InlineKeyboardMarkup? markup, string? result)> Handle(IMessageTranslationManger messageTranslationManger, UpdateContainer container)
        {
            var answerTag = await messageTranslationManger
                .GetTagByText(CollectionName.CommonTranslations, container.Update!.CallbackQuery!.Data!);
            if (answerTag == TextTag.DoneBtn)
            {
                var minHour = container.Update!.CallbackQuery.Message.ReplyMarkup
                    .InlineKeyboard.ElementAt(1).ElementAt(0).Text
                    .Substring(0, 5)
                    .Trim();
                return (null, minHour);
            }
            
            var minText = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.Minutes);
            var hourText = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.Hour);
            var doneBtn = await messageTranslationManger
               .GetTextByTag(CollectionName.CommonTranslations, TextTag.DoneBtn);
            var message = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.SelectTime);
            var hours = Int16.Parse(container.Update!.CallbackQuery.Data.Split(":")[0]);
            var mins = Int16.Parse(container.Update!.CallbackQuery.Data.Split(":")[1]);

            var buttons = new List<List<InlineKeyboardButton>>() {
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton("🔺 5 " + minText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours, mins + 5)
                    },
                    new InlineKeyboardButton("🔺 10 " + minText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours, mins + 10)
                    },
                    new InlineKeyboardButton("🔺 1 " + hourText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours + 1, 0)
                    }
                },
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton(container.Update!.CallbackQuery.Data + " " + doneBtn)
                    {
                        CallbackData = doneBtn
                    },
                },
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton("🔻 5 " + minText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours, mins - 5)
                    },
                    new InlineKeyboardButton("🔻 10 " + minText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours, mins - 10)
                    },
                    new InlineKeyboardButton("🔻 1 " + hourText)
                    {
                        CallbackData = ConvertHoursAndMinutesInStr(hours - 1, 0)
                    }
                }
            };
            
            var markup = new InlineKeyboardMarkup(buttons);
            return (markup, null);
        }
        
        private static string ConvertHoursAndMinutesInStr(int hours, int minutes)
        {
            if (hours > 23)
            {
                hours = 0 + hours - 23;
                minutes = 0;
            }
            if (minutes < 0)
            {
                minutes = 60 + minutes;
                hours -= 1;
            }
            if (hours < 0)
            {
                hours = 0 + 23 + hours;
                minutes = 0;
            }
            if (minutes > 59)
            {
                minutes = 0;
                hours += 1;
            }
            var minutesStr = minutes.ToString().Count() < 2 ? "0" + minutes.ToString() : minutes.ToString();
            return hours.ToString() + ":" + minutesStr;
        }
    }
}
