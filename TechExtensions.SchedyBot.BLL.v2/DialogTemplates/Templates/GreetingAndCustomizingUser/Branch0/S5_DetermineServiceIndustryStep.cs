using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0;

public class S5_DetermineServiceIndustryStep : IDialogStep
{
    private readonly IBotMessageManager _botMessageSender;
    private readonly UpdateContainer _container;
    private readonly BotReplyKeyboardMarkupHandler _markupHandler;


    private readonly IMessageTranslationManger _messageTranslationManger;
    private readonly IServiceAndSpecService _serviceAndSpecService;

    private readonly IServiceService _serviceService;
    private readonly string _errorMessage = "branch0step5err";
    private readonly string _messageTag = "branch0step5mess";

    public S5_DetermineServiceIndustryStep(IMessageTranslationManger messageTranslationManger,
        IBotMessageManager botMessageSender,
        IServiceAndSpecService serviceAndSpecService,
        IServiceService serviceService, 
        UpdateContainer container,
        BotReplyKeyboardMarkupHandler markupHandler)
    {
        _serviceAndSpecService = serviceAndSpecService;
        _serviceService = serviceService;
        _container = container;
        _markupHandler = markupHandler;
        _messageTranslationManger = messageTranslationManger;
        _botMessageSender = botMessageSender;
    }

    public async Task SendReplyToUser()
    {

        var message = await _messageTranslationManger.GetTextByTag(_container.Template!.TranslationCollectionName, _messageTag);
        var buttons = await _messageTranslationManger.GetAllTextsFromCollection(CollectionName.ServiceIndustryCollection);
        var markup = await _markupHandler.FormReplyKeyboardMarkupFromButtons(buttons, true, true);
        await _botMessageSender.Send(message, markup);
    }

    public async Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
    {
        // var serviceKind = await _serviceKindService.GetByName(clientAnswer);
        // if (serviceKind is null)
        // {
        //     serviceKind = new ServiceKind{ KindNameTag = clientAnswer};
        //     await _serviceKindService.Create(serviceKind);
        // }
        //
        // await _serviceAndSpecService.CreateByServiceKind(serviceKind, _container.Client!);
        return this.NextStep();
    }
    public bool LastStep { get; set; } = false;
    public int TemplateId { get; } = 0;
    public int BranchId { get; } = 0;
    public int StepId { get; } = 5;
}