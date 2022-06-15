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
    public static class TimeSpanInlineKeyboard
    {
        public static async Task<IReplyMarkup> Launch(IMessageTranslationManger messageTranslationManger)
        {
        
            var minText = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.Minutes);
            var hourText = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.Hour);
            var doneBtn = await messageTranslationManger
               .GetTextByTag(CollectionName.CommonTranslations, TextTag.DoneBtn);
            
            var buttons = new List<List<InlineKeyboardButton>>() {
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton("🔺 5 " + minText)
                    {
                        CallbackData = "00:05"
                    },
                    new InlineKeyboardButton("🔺 10 " + minText)
                    {
                        CallbackData = "00:10"
                    },
                    new InlineKeyboardButton("🔺 1 " + hourText)
                    {
                        CallbackData = "01:00"
                    }
                },
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton("00 " + hourText + " 00 " + minText + " " + doneBtn)
                    {
                        CallbackData = doneBtn
                    },
                },
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton("🔻 5 " + minText)
                    {
                        CallbackData = "02:55"
                    },
                    new InlineKeyboardButton("🔻 10 " + minText)
                    {
                        CallbackData = "02:50"
                    },
                    new InlineKeyboardButton("🔻 1 " + hourText)
                    {
                        CallbackData = "02:00"
                    }
                },
            };
            
            var markup = new InlineKeyboardMarkup(buttons);
            return markup;
        }
        
        public static async Task<(InlineKeyboardMarkup? markup, string? result)> Handle(IMessageTranslationManger messageTranslationManger, UpdateContainer container)
        {
            var answerTag = await messageTranslationManger
                .GetTagByText(CollectionName.CommonTranslations, container.Update!.CallbackQuery.Data);
            if (answerTag == TextTag.DoneBtn)
            {
                var minHourArr = container.Update!.CallbackQuery.Message.ReplyMarkup
                    .InlineKeyboard.ElementAt(1).ElementAt(0).Text
                    .Split(" ");
                var minHour = minHourArr[0] + ":" + minHourArr[2];
                return (null, minHour);
            }
            
            var minText = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.Minutes);
            var hourText = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.Hour);
            var doneBtn = await messageTranslationManger
               .GetTextByTag(CollectionName.CommonTranslations, TextTag.DoneBtn);
            var message = await messageTranslationManger
                .GetTextByTag(CollectionName.CommonTranslations, TextTag.SelectTimeSpan);
            
            var hours = int.Parse(container.Update!.CallbackQuery.Data.Split(":")[0]);
            var mins = int.Parse(container.Update!.CallbackQuery.Data.Split(":")[1]);
        
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
                    new InlineKeyboardButton(hours + " "+ hourText  + " " + mins  + " " + minText + " " + doneBtn)
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
            if (minutes < 0)
            {
                minutes = 60 + minutes;
                hours -= 1;
            }
            if (hours < 0)
            {
                hours = 0;
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
