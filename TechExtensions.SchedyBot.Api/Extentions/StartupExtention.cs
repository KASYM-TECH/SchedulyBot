using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.SchedyBot.DLL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch0;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch1;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch0;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0;
using TechExtensions.SchedyBot.BLL.v2.Services;
using TechExtensions.SchedyBot.BLL.v2.Services.CurrentDialogNavigator;
using TechExtensions.SchedyBot.BLL.v2.Services.CurrentDialogNavigator.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler;
using TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler.DI;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch1;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch2;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.TelegramBotLinkModule;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch1;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch3;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingManagment.DialogSteps.Branch2;

namespace TechExtensions.SchedyBot.Api.Extentions
{
    public static class StartupExtentions  
    {
        public static void AddV2Services(this IServiceCollection serviceCollection)
        {
            // Scoped
            serviceCollection.AddScoped<IUpdateHandler, UpdateHandler>();
            
            serviceCollection.AddScoped<ICurrentDialogNavigator, CurrentDialogNavigator>();
            serviceCollection.AddScoped<IUpdateMessageService, UpdateMessageService>();
            serviceCollection.AddScoped<IBotMessageManager, BotMessageManager>();
            serviceCollection.AddScoped<IMessageTranslationManger, MessageTranslationManger>();
            
            // Entity Services
            serviceCollection.AddScoped<IBookingInConfirmationService, BookingInConfirmationService>();
            serviceCollection.AddScoped<IBookingService, BookingService>();
            serviceCollection.AddScoped<IClientService, ClientService>();
            serviceCollection.AddScoped<IServiceService, ServiceService>();
            serviceCollection.AddScoped<ICurrentDialogService, CurrentDialogService>();
            serviceCollection.AddScoped<IScheduleService, ScheduleService>();
            serviceCollection.AddScoped<IServiceAndSpecService, ServiceAndSpecService>();
            serviceCollection.AddScoped<IServiceService, ServiceService>();
            serviceCollection.AddScoped<IStepService, StepService>();

            serviceCollection.AddScoped<UpdateContainer>();
            serviceCollection.AddScoped<TemplateService>();
            serviceCollection.AddScoped<BotReplyKeyboardMarkupHandler>();
            serviceCollection.AddScoped<TelegramBotLinkManager>();
            serviceCollection.AddScoped<CustomTemplateManager>();
            serviceCollection.AddScoped<MainMenuManager>();
            serviceCollection.AddScoped<HangFireService>();

            // Templates
            serviceCollection.AddScoped<IDialogTemplate, GreetingAndCustomizingUserTemplate>();
            // serviceCollection.AddScoped<IDialogTemplate, BookingNotificationForSellerTemplate>();
            serviceCollection.AddScoped<IDialogTemplate, BookingManagmentTemplate>();
            serviceCollection.AddScoped<IDialogTemplate, BookSellerTimeTemplate>();
            // serviceCollection.AddScoped<IDialogTemplate, RegisterUserWhileAttemptingToBookTemplate>();
            
            // Steps
            // GreetingAndCustomizingUserTemplate
            serviceCollection.AddScoped<IDialogStep, S0_GreetingStep>();
            serviceCollection.AddScoped<IDialogStep, S1_NameInputStep>();
            serviceCollection.AddScoped<IDialogStep, S2_LastNameInputStep>();
            serviceCollection.AddScoped<IDialogStep, BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0.S3_TimezoneInputStep>();
            serviceCollection.AddScoped<IDialogStep, S4_WannaCreateScheduleStep>();
            serviceCollection.AddScoped<IDialogStep, S5_DetermineServiceIndustryStep>();
            serviceCollection.AddScoped<IDialogStep, S6_SelectServiceStep>();
            serviceCollection.AddScoped<IDialogStep, S7_ServicePriceInputStep>();
            serviceCollection.AddScoped<IDialogStep, S8_SelectWeekDaysStep>();
            serviceCollection.AddScoped<IDialogStep, S9_DetermineScheduleTimeFromStep>();
            serviceCollection.AddScoped<IDialogStep, S10_DetermineScheduleTimeToStep>();
            serviceCollection.AddScoped<IDialogStep, S11_DetermineDurationStep>();
            serviceCollection.AddScoped<IDialogStep, S12_DevoteSomeTimeToLunchStep>();
            serviceCollection.AddScoped<IDialogStep, S13_DetermineLunchTimeFromStep>();
            serviceCollection.AddScoped<IDialogStep, S14_DetermineLunchTimeToStep>();
            serviceCollection.AddScoped<IDialogStep, S15_EndOfTheZeroBranchStep>();
            serviceCollection.AddScoped<IDialogStep, EndOfTheFirstBranchStep>();
            serviceCollection.AddScoped<IDialogStep, EndOfTheSecondBranchStep>();

            // BookSellerTimeTemplate
            serviceCollection.AddScoped<IDialogStep, S0_LetsBookTimeStep>();
            serviceCollection.AddScoped<IDialogStep, S1_ChooseServiceStep>();
            serviceCollection.AddScoped<IDialogStep, S2_SelectDateAndTimeStep>();
            serviceCollection.AddScoped<IDialogStep, S3_AttachMessageIfNeedStep>();
            serviceCollection.AddScoped<IDialogStep, S0_QuickRegistrationStarts>();
            serviceCollection.AddScoped<IDialogStep, S1_InputNameStep>();
            serviceCollection.AddScoped<IDialogStep, S2_InputLastNameStep>();
            serviceCollection.AddScoped<IDialogStep, S4_NowYouCanBookTimeStep>();
            serviceCollection.AddScoped<IDialogStep, S4_SuccessfullyBookedStep>();
            serviceCollection.AddScoped<IDialogStep, BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch1.S3_TimezoneInputStep>();

            // BookingManagmentTemplate
            serviceCollection.AddScoped<IDialogStep, S0_ShowIncomingBookingsStep>();
            serviceCollection.AddScoped<IDialogStep, S2_ManageSelectedBookingStep>();
            serviceCollection.AddScoped<IDialogStep, S3_AttachCommentToActionStep>();
            serviceCollection.AddScoped<IDialogStep, S0_ChangeTimeOnActionStep>();
            serviceCollection.AddScoped<IDialogStep, S4_IncomeBookingActionHandleStep>();
            serviceCollection.AddScoped<IDialogStep, S0_OutcomeChangeTime>();
            serviceCollection.AddScoped<IDialogStep, S0_SelectOutgoingBookingStep>();
            serviceCollection.AddScoped<IDialogStep, S1_ManageSelectedBookingByClientStep>();
            serviceCollection.AddScoped<IDialogStep, S2_AttachCommentToActionStep>();
            serviceCollection.AddScoped<IDialogStep, S3_OutcomeBookingActionHandleStep>();
            serviceCollection.AddScoped<IDialogStep, S4_NowYouCanBookTimeStep>();
        }
        
        public static void AddScopedRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IDbRepository<Address>, AddressRepository>();
            serviceCollection.AddScoped<IDbRepository<Booking>, BookingRepository>();
            serviceCollection.AddScoped<IDbRepository<Contact>, ContactRepository>();
            serviceCollection.AddScoped<IDbRepository<Review>, ReviewRepository>();
            serviceCollection.AddScoped<IDbRepository<Schedule>, ScheduleRepository>();
            serviceCollection.AddScoped<IDbRepository<CurrentDialog>, CurrentDialogRepository>();
            serviceCollection.AddScoped<IDbRepository<Service>, ServiceRepository>();
            serviceCollection.AddScoped<IDbRepository<ServiceAndSpec>, ServiceAndSpecRepository>();
            serviceCollection.AddScoped<IDbRepository<UpdateMessage>, UpdateMessageRepository>();
            serviceCollection.AddScoped<IDbRepository<Client>, ClientRepository>();
            serviceCollection.AddScoped<IDbRepository<BookingInConfirmation>, BookingInConfirmationRepository>();
            serviceCollection.AddScoped<IDbRepository<Step>, StepRepository>();
        }
    }
}
