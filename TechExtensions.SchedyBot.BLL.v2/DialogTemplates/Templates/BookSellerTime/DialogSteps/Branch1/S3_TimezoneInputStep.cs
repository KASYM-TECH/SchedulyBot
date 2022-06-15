using GeoTimeZone;
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
using TimeZoneConverter;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch1
{
    public class S3_TimezoneInputStep : IDialogStep
    {
        private string _messageTag = "branch0step3mess";
        private string _answerTag = "branch0step3mess1";
        private string _errorMessage = "timeZoneErr";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IClientService _userService;
        private readonly UpdateContainer _container;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;

        public S3_TimezoneInputStep(IMessageTranslationManger messageTranslationManger, IClientService userService, IBotMessageManager botMessageSender, UpdateContainer container, BotReplyKeyboardMarkupHandler markupHandler)
        {
            _userService = userService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _container = container;
            _markupHandler = markupHandler;
        }
        public async Task SendReplyToUser()
        {
            var message = await _messageTranslationManger.GetTextByTag(CollectionName.GreetingAndCustomizingUserTemplate, _messageTag);
            var markup = (await _markupHandler.FormReplyKeyboardMarkupFromButtons(null, true, true))
                // TODO: Добавить в параметры строку на нужном языке
                .WithLocationRequest();

            await _botMessageSender.Send(message, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            // TODO: Сделать еще возможность определять таймзон с помощью ручного ввода
            var location = _container.Update?.Message?.Location;
            if (location == null)
            {
                await _botMessageSender.Send(
                    await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _errorMessage));
                return this.CurrentStep();
            }

            // TODO: Сделать UTC сервис
            var tzIana = TimeZoneLookup.GetTimeZone(location.Latitude, location.Longitude).Result;
            var tzInfo = TZConvert.GetTimeZoneInfo(tzIana);
            var convertedTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tzInfo);

            //TODO: TimeZoneGuard, но вообще надо ли
            await _botMessageSender.Send(
                await _messageTranslationManger.GetTextByTag(CollectionName.GreetingAndCustomizingUserTemplate,
                    _answerTag));

            _container.Client!.TimeZoneOffset = tzInfo.BaseUtcOffset;
            await _userService.Update(_container.Client);

            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int)TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 1;
        public int StepId { get; } = 3;
    }
}
