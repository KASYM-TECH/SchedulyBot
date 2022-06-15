using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections
{
    public class SelectServiceInlineKeyBoard : ISelectServiceInlineKeyBoard
    {
        
        private readonly IBotMessageManager _botMessageManager;
        private readonly IBookingService _bookingService;
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IServiceAndIndustryRelationManager _serviceAndIndustryRelationManager;
        private readonly UpdateContainer _updateContainer;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly IServiceService _serviceService;
        
        public SelectServiceInlineKeyBoard(IBookingService bookingService,
            IMessageTranslationManger messageTranslationManger,
            UpdateContainer updateContainer,
            IServiceService serviceService,
            IServiceAndSpecService serviceAndSpecService,
            IServiceAndIndustryRelationManager serviceAndIndustryRelationManager,
            IBotMessageManager botMessageManager)
        {
            _serviceService = serviceService;
            _serviceAndSpecService = serviceAndSpecService;
            _serviceAndIndustryRelationManager = serviceAndIndustryRelationManager;
            _messageTranslationManger = messageTranslationManger;
            _updateContainer = updateContainer;
            _botMessageManager = botMessageManager;
            _bookingService = bookingService;
        }
        // public async Task Launch(CurrentDialog currentDialog, bool onlyServicesOfExecutor = false)
        // {
        //     await SendScopeOfServices(currentDialog, onlyServicesOfExecutor);
        //     currentDialog.CurrentInlineKeyboard = typeof(SelectServiceInlineKeyBoard)
        //         .ToString().Split(".").Last();
        //     currentDialog.State = CurrentDialogState.BeingUsedByInlineKeyboard;
        // }
        //
        // public async Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update, bool onlyServicesOfExecutor = false)
        // {
        //     var inlineBtn = update.CallbackQuery.Message.ReplyMarkup.InlineKeyboard
        //         .FirstOrDefault(k => k.Any(k1 => k1.CallbackData == update.CallbackQuery.Data))
        //         .FirstOrDefault(k1 => k1.CallbackData == update.CallbackQuery.Data);
        //     var answerBtnMessageTag = await _messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, inlineBtn.Text);
        //     var answerBtnDataTag = await _messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, update.CallbackQuery.Data);
        //
        //     var isThisServiceTag = await _serviceAndIndustryRelationManager.GetIndustryTagByServiceTag(update.CallbackQuery.Data) is not null;
        //     var isThisIndustryTag = await _serviceAndIndustryRelationManager.GetAllServiceTagsByIndustryTag(update.CallbackQuery.Data) is not null;
        //     if (answerBtnMessageTag == TextTag.GetBackBtn && inlineBtn.CallbackData == TextTag.SelectServiceKind)
        //     {
        //         await handleKeyboardExit(currentDialog, update.CallbackQuery);
        //         return currentDialog;
        //     }
        //     if (answerBtnMessageTag == TextTag.GetBackBtn && inlineBtn.CallbackData == TextTag.SelectService)
        //     {
        //         await SendScopeOfServices(currentDialog, onlyServicesOfExecutor);
        //         return currentDialog;
        //     }
        //     if (isThisIndustryTag)
        //         await SendServices(currentDialog, update.CallbackQuery.Data, onlyServicesOfExecutor);
        //     if (isThisServiceTag)
        //     {
        //         await handleShosenService(currentDialog, update.CallbackQuery);
        //         currentDialog.CurrentInlineKeyboard = null;
        //         await _botMessageManager.DeleteInlineKeyboard(update.CallbackQuery.Message.MessageId);
        //     }
        //     return currentDialog;
        // }
        //
        // private async Task SendScopeOfServices(CurrentDialog currentDialog, bool onlyServicesOfExecutor)
        // {
        //     var serviceIndustries = new List<string>();
        //     if(onlyServicesOfExecutor)
        //     {
        //         var uncompletedBooking = _bookingService.GetBooking(b => b.Client.Id == currentDialog.Client.Id
        //         && b.Status == BookingCompletionStatus.Initiated);
        //         var services = _serviceService.GetServices(s => s.IsActive);
        //         var serviceKinds = _serviceKindService.GetServices(s => s.IsActive);
        //         serviceIndustries = _serviceAndSpecService
        //             .GetManyServiceAndSpec(i => i.User.Id == uncompletedBooking.Executor.Id)
        //             .Select(i => i.Service.ServiceKind.KindNameTag).Distinct().ToList();
        //         var translatedIndustries = new List<string>();
        //         foreach (var industry in serviceIndustries)
        //             translatedIndustries.Add( await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.ServiceIndustryCollection, industry));
        //         serviceIndustries = translatedIndustries;
        //     }
        //     else
        //         serviceIndustries = await _messageTranslationManger
        //             .GetAllTextsFromCollection(CollectionName.ServiceIndustryCollection);
        //     var buttons = new List<List<InlineKeyboardButton>>();
        //     var serviceIndustriesChunks = serviceIndustries.Chunk(2);
        //
        //     var index = 0;
        //     foreach (var serviceIndustryChank in serviceIndustriesChunks)
        //     {
        //         buttons.Add(new List<InlineKeyboardButton>());
        //         foreach (var erviceIndustry in serviceIndustryChank)
        //         {
        //             buttons[index].Add(new InlineKeyboardButton(erviceIndustry)
        //             {
        //                 CallbackData =
        //                 await _messageTranslationManger.GetTagByText(CollectionName.ServiceIndustryCollection, erviceIndustry)
        //             });
        //         }
        //         index++;
        //     }
        //     var backBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn);
        //     buttons.Add(new List<InlineKeyboardButton>() { new InlineKeyboardButton(backBtn) { CallbackData = TextTag.SelectServiceKind } });
        //     var markup = new InlineKeyboardMarkup(buttons);
        //     var message = await _messageTranslationManger
        //         .GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.SelectServiceKind);
        //
        //     if (_updateContainer.Update.CallbackQuery == null)
        //         await _botMessageManager.SendInlineKeyboard(message, markup);
        //     else
        //         await _botMessageManager.EditInlineKeyboard(_updateContainer.Update.CallbackQuery.Message.MessageId, 
        //            message, markup);
        // }
        // private async Task SendServices(CurrentDialog currentDialog, string serviceIndustryTag, bool onlyServicesOfExecutor)
        // {
        //     var serviceTags = new List<string>();
        //     if (onlyServicesOfExecutor)
        //     {
        //         var uncompletedBooking = _bookingService.GetBooking(b => b.Client.Id == currentDialog.Client.Id
        //         && b.Status == BookingCompletionStatus.Initiated);
        //         var services = _serviceService.GetServices(s => s.IsActive);
        //         var serviceKinds = _serviceKindService.GetServices(s => s.IsActive);
        //         serviceTags = _serviceAndSpecService
        //             .GetManyServiceAndSpec(i => i.User.Id == uncompletedBooking.Executor.Id && i.Service.ServiceKind.KindNameTag == serviceIndustryTag)
        //             .Select(i => i.Service.NameTag).Distinct().ToList();
        //     }
        //     else
        //         serviceTags = await _serviceAndIndustryRelationManager.GetAllServiceTagsByIndustryTag(serviceIndustryTag);
        //
        //     var buttons = new List<List<InlineKeyboardButton>>();
        //     var serviceTagChunks = serviceTags.Chunk(2);
        //
        //     var index = 0;
        //     foreach (var serviceTagChunk in serviceTagChunks)
        //     {
        //         buttons.Add(new List<InlineKeyboardButton>());
        //         foreach (var serviceTag in serviceTagChunk)
        //         {
        //             buttons[index].Add(new InlineKeyboardButton(await _messageTranslationManger
        //                 .GetTranslatedTextByTag(CollectionName.ServiceCollection, serviceTag))
        //             { CallbackData = serviceTag });
        //         }
        //         index++;
        //     }
        //
        //     var backBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn);
        //     buttons.Add(new List<InlineKeyboardButton>() { new InlineKeyboardButton(backBtn) { CallbackData = TextTag.SelectService } });
        //     var markup = new InlineKeyboardMarkup(buttons);
        //     var message = await _messageTranslationManger
        //         .GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.SelectService);
        //     if (_updateContainer.Update.CallbackQuery == null)
        //         await _botMessageManager.SendInlineKeyboard(message, markup);
        //     else
        //         await _botMessageManager.EditInlineKeyboard(_updateContainer.Update.CallbackQuery.Message.MessageId,
        //              message, markup);
        // }
        public ISelectServiceInlineKeyBoard.HandleMethod handleShosenService { get; set; }
        public ISelectServiceInlineKeyBoard.HandleMethod handleKeyboardExit { get; set; }
        public Task Launch(CurrentDialog currentDialog, bool onlyServicesOfExecutor = false)
        {
            throw new NotImplementedException();
        }

        public Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update, bool onlyServicesOfExecutor = false)
        {
            throw new NotImplementedException();
        }
    }
}
