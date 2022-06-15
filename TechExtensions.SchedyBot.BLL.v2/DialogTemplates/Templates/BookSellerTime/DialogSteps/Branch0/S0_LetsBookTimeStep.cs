using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch0
{
    public class S0_LetsBookTimeStep : IDialogStep
    {
        private string _messageTag = "branch0step0mess";
        private string _errorMessage = TextTag.BtnErr;
        private string _buttonTag = TextTag.GreateBtn;

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly IClientService _clientService;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;
        private readonly UpdateContainer _container;
        
        public S0_LetsBookTimeStep(IMessageTranslationManger messageTranslationManger, 
            IBookingService bookingService,
            IBotMessageManager botMessageSender, 
            BotReplyKeyboardMarkupHandler markupHandler, UpdateContainer container, IClientService clientService)
        {
            _bookingService = bookingService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
            _markupHandler = markupHandler;
            _container = container;
            _clientService = clientService;
        }

        public async Task SendReplyToUser()
        {


            // Инициализируем первичную модель бронирования
            // TODO: Обработка ошибки, если больше одного параметра или наименование параметра не clientId
            var startParameter = _container.Update.Message.Text.Split(' ')[1];
            var executorId = int.Parse(startParameter.Replace("executorId", ""));
            // TODO: Обработка сценария, если нет клиента с таким Id + если это id покупателя (сам у себя пытается забронировать)
            var executor = await _clientService.GetById(executorId);
            var booking = new Booking
            {
                ClientId = _container.Client!.Id,
                Client = _container.Client,
                Executor = executor,
                ExecutorId = executorId,
                Status = BookingCompletionStatus.New
            };
            await _bookingService.Create(booking);
            
            var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
            var formattedMessage = string.Format(message, string.Join(executor.FirstName, " ", executor.LastName));
            var buttonText = await _messageTranslationManger.GetTextByTag(CollectionName.CommonTranslations, _buttonTag);
            var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(new List<string>{buttonText}, false, true);
            await _botMessageSender.Send(formattedMessage, markup);
        }

        public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        { 
            return this.NextStep();
        }
        public bool LastStep { get; set; } = false;
        public int TemplateId { get; } = (int) TemplateEnum.BookSellerTimeTemplate;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 0;
    }
}
