using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections
{
    public class SelectDateInlineKeyboard : ISelectDateInlineKeyboard
    {
        // private readonly IBotMessageManager _botMessageManager;
        // private readonly IMessageTranslationManger _messageTranslationManger;
        // private readonly IScheduleService _scheduleService;
        // private readonly IBookingService _bookingService;
        // private readonly IUpdateContainer _updateContainer;
        // private List<DateTime> PossibleDates;
        //
        // public CurrentDialog CurrentDialog { get; set; }
        // public ISelectDateInlineKeyboard.HandleMethod handleDate { get; set; }
        // public ISelectDateInlineKeyboard.HandleMethod handleKeyboardExit { get; set; }
        // public SelectDateInlineKeyboard(
        //     IUpdateContainer updateContainer,
        //     IBotMessageManager botMessageManager,
        //     IScheduleService scheduleService,
        //     IBookingService bookingService,
        //     IMessageTranslationManger messageTranslationManger)
        // {
        //     _updateContainer = updateContainer;
        //     _bookingService = bookingService;
        //     _scheduleService = scheduleService;
        //     _botMessageManager = botMessageManager;
        //     _messageTranslationManger = messageTranslationManger;
        // }
        //
        // public async Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update)
        // {
        //     CurrentDialog = currentDialog;
        //     SetPossibleDatesAccordingToSellerSchedule();
        //     switch (update.CallbackQuery?.Data)
        //     {
        //         case TextTag.GetBackBtn:
        //             await handleKeyboardExit(currentDialog, update.CallbackQuery);
        //             return currentDialog;
        //         case TextTag.Next:
        //             await SendNextScopeOfDays(update.CallbackQuery);
        //             return currentDialog;
        //         case TextTag.Previous:
        //             await SendPreviousScopeOfDays(update.CallbackQuery);
        //             return currentDialog;
        //         default:
        //             await handleDate(currentDialog, update.CallbackQuery);
        //             await _botMessageManager.DeleteInlineKeyboard(update.CallbackQuery.Message.MessageId);
        //             return currentDialog;
        //     }
        // }
        // private async Task SendPreviousScopeOfDays(CallbackQuery callbackQuery)
        // {
        //     var lowestDate = callbackQuery.Message?.ReplyMarkup?.InlineKeyboard.ElementAt(0)
        //         .FirstOrDefault();
        //     var dateNow = DateTime.UtcNow;
        //     var indexOflowestDate = PossibleDates.IndexOf(DateTime.Parse(lowestDate.CallbackData));
        //     if(PossibleDates[indexOflowestDate - 4].ToShortDateString() == dateNow.ToShortDateString())
        //     {
        //         await _botMessageManager.EditInlineKeyboard(callbackQuery.Message.MessageId, callbackQuery.Message.Text, GetLunchMarkup().Result);
        //         return;
        //     }
        //     var buttons = new List<List<InlineKeyboardButton>>();
        //     for (int i = 4; i > 0; i--)
        //     {
        //         var inlineKeybr = new List<InlineKeyboardButton>()
        //         {
        //             new InlineKeyboardButton(_messageTranslationManger.GetTranslatedTextByTag(CollectionName.DaysOfWeek,
        //             PossibleDates[indexOflowestDate - i].DayOfWeek.ToString()).Result + " " +  PossibleDates[indexOflowestDate - i].ToShortDateString())
        //             {
        //                 CallbackData = PossibleDates[indexOflowestDate - i].ToString()
        //             }
        //         };
        //         buttons.Add(inlineKeybr);
        //     }
        //     var backBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn);
        //     var nextBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.Next);
        //     var previousBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.Previous);
        //     buttons.Add(new List<InlineKeyboardButton>() { new InlineKeyboardButton(backBtn) { CallbackData = TextTag.GetBackBtn },
        //         new InlineKeyboardButton(previousBtn) { CallbackData = TextTag.Previous },
        //         new InlineKeyboardButton(nextBtn) { CallbackData = TextTag.Next } });
        //     var markup = new InlineKeyboardMarkup(buttons);
        //     await _botMessageManager.EditInlineKeyboard(callbackQuery.Message.MessageId, callbackQuery.Message.Text, markup);
        //     return;
        // }
        //
        // private async Task SendNextScopeOfDays(CallbackQuery callbackQuery)
        // {
        //     var highestDate = callbackQuery.Message?.ReplyMarkup?.InlineKeyboard.ElementAt(3)
        //         .FirstOrDefault();
        //     var indexOfHighestDate = PossibleDates.IndexOf(DateTime.Parse(highestDate.CallbackData));
        //
        //     var buttons = new List<List<InlineKeyboardButton>>();
        //     for (int i = 0; i<4; i++)
        //     {
        //         var inlineKeybr = new List<InlineKeyboardButton>()
        //         {
        //             new InlineKeyboardButton(_messageTranslationManger.GetTranslatedTextByTag(CollectionName.DaysOfWeek,
        //             PossibleDates[indexOfHighestDate + i+1].DayOfWeek.ToString()).Result + " " + 
        //             PossibleDates[indexOfHighestDate + i+1].ToShortDateString())
        //             {
        //                 CallbackData = PossibleDates[indexOfHighestDate + i+1].ToString()
        //             }
        //         };
        //         buttons.Add(inlineKeybr);
        //     }
        //
        //     var backBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn);
        //     var nextBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.Next);
        //     var previousBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.Previous);
        //     buttons.Add(new List<InlineKeyboardButton>() {
        //         new InlineKeyboardButton(backBtn) { CallbackData = TextTag.GetBackBtn },
        //         new InlineKeyboardButton(previousBtn) { CallbackData = TextTag.Previous },
        //         new InlineKeyboardButton(nextBtn) { CallbackData = TextTag.Next } 
        //          });
        //     var markup = new InlineKeyboardMarkup(buttons);
        //     var message = await _messageTranslationManger
        //         .GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.SelectDate);
        //     await _botMessageManager.EditInlineKeyboard(callbackQuery.Message.MessageId, message, markup);
        // }
        // public async Task Launch(CurrentDialog currentDialog)
        // {
        //     CurrentDialog = currentDialog;
        //     SetPossibleDatesAccordingToSellerSchedule();
        //     var message = await _messageTranslationManger
        //         .GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.SelectDate);
        //     await _botMessageManager.SendInlineKeyboard(message, GetLunchMarkup().Result);
        //     currentDialog.CurrentInlineKeyboard = typeof(SelectDateInlineKeyboard)
        //         .ToString().Split(".").Last();
        //     currentDialog.State = CurrentDialogState.BeingUsedByInlineKeyboard;
        // }
        // private async Task<InlineKeyboardMarkup> GetLunchMarkup()
        // {
        //     var dateNow = DateTime.UtcNow;
        //
        //     var buttons = new List<List<InlineKeyboardButton>>() {
        //         new List<InlineKeyboardButton>()
        //         {
        //             new InlineKeyboardButton(_messageTranslationManger.GetTranslatedTextByTag(CollectionName.DaysOfWeek,
        //             PossibleDates.First().DayOfWeek.ToString()).Result + " - " + PossibleDates.First().ToShortDateString())
        //             {
        //                 CallbackData =  PossibleDates.First().ToString()
        //             }
        //         },
        //         new List<InlineKeyboardButton>()
        //         {
        //             new InlineKeyboardButton(_messageTranslationManger.GetTranslatedTextByTag(CollectionName.DaysOfWeek,
        //             PossibleDates[1].DayOfWeek.ToString()).Result + " - " +  PossibleDates[1].ToShortDateString())
        //             {
        //                 CallbackData =  PossibleDates[1].ToString()
        //             },
        //         },
        //         new List<InlineKeyboardButton>()
        //         {
        //             new InlineKeyboardButton(_messageTranslationManger.GetTranslatedTextByTag(CollectionName.DaysOfWeek,
        //             PossibleDates[2].DayOfWeek.ToString()).Result + " - " + PossibleDates[2].ToShortDateString())
        //             {
        //                 CallbackData =  PossibleDates[2].ToString()
        //             },
        //         },
        //         new List<InlineKeyboardButton>()
        //         {
        //             new InlineKeyboardButton(_messageTranslationManger.GetTranslatedTextByTag(CollectionName.DaysOfWeek,
        //             PossibleDates[3].DayOfWeek.ToString()).Result + " - " + PossibleDates[3].ToShortDateString())
        //             {
        //                 CallbackData = PossibleDates[3].ToString()
        //             },
        //         },
        //     };
        //     var backBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn);
        //     var nextBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.Next);
        //     buttons.Add(new List<InlineKeyboardButton>() { new InlineKeyboardButton(backBtn) { CallbackData = TextTag.GetBackBtn },
        //         new InlineKeyboardButton(nextBtn) { CallbackData = TextTag.Next } });
        //     var markup = new InlineKeyboardMarkup(buttons);
        //     return markup;
        // }
        //
        // private void SetPossibleDatesAccordingToSellerSchedule()
        // {
        //     var callbackQuery = _updateContainer.Update?.CallbackQuery;
        //     PossibleDates = new List<DateTime>();
        //     var allFreeWeekDaysOfSeller = GetAllFreeWeekDaysOfSeller();
        //
        //     var pointer = -10;
        //     var patience = 7;
        //     if (CurrentDialog.CurrentInlineKeyboard != typeof(SelectDateInlineKeyboard).ToString().Split(".").Last()
        //         || CurrentDialog.CurrentInlineKeyboard == null)
        //     {
        //         var middleDate = DateTime.UtcNow;
        //         for (int i = 0; i < 5; i++)
        //         {
        //             while (patience != 0 && (allFreeWeekDaysOfSeller.Contains(middleDate.DayOfWeek.ToString()) is false))
        //             {
        //                 middleDate = middleDate.AddDays(1);
        //                 patience--;
        //             }
        //             if(patience != 0)
        //                 PossibleDates.Add(middleDate);
        //             middleDate = middleDate.AddDays(1);
        //         }
        //     }
        //     else
        //     {
        //         var middleDate = DateTime.Parse(callbackQuery.Message?.ReplyMarkup?.InlineKeyboard?.ElementAt(2)!
        //             .FirstOrDefault()!.CallbackData!);
        //         for (int i = -10; i < 10; i++)
        //         {
        //             var trialDate = middleDate.AddDays(pointer);
        //             while (patience != 0 && allFreeWeekDaysOfSeller.Contains(trialDate.DayOfWeek.ToString()) is false)
        //             {
        //                 pointer++;
        //                 trialDate = trialDate.AddDays(1);
        //                 patience--;
        //             }
        //             PossibleDates.Add(trialDate);
        //             pointer++;
        //         }
        //     }
        // }
        // private List<string> GetAllFreeWeekDaysOfSeller()
        // {
        //     var initiatedBooking = _bookingService.GetBooking(b => b.Status == BookingCompletionStatus.Initiated
        //     && b.Client.Id == CurrentDialog.Client.Id);
        //     var allSchedulesOfThisClient = _scheduleService.GetMany(s => s.ClientId == initiatedBooking.Executor.Id)
        //         .Select(s => s.WeekDay.ToString())
        //         .Distinct()
        //         .ToList();       
        //     return allSchedulesOfThisClient;
        // }
        public CurrentDialog CurrentDialog { get; set; }
        public ISelectDateInlineKeyboard.HandleMethod handleKeyboardExit { get; set; }
        public ISelectDateInlineKeyboard.HandleMethod handleDate { get; set; }
        public Task Launch(CurrentDialog currentDialog)
        {
            throw new NotImplementedException();
        }

        public Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update)
        {
            throw new NotImplementedException();
        }
    }
}
