using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.Extentions;
using TechExtensions.SchedyBot.DLL.Entities;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.DialogTemplates.DialogServices.DI;
using TechExtensions.SchedyBot.BLL.Models;
using TechExtensions.SchedyBot.BLL.Services.Abstractions;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.UpdateManagers;

/// <summary>
/// Менеджер по работе с командами от пользователя
/// </summary>
public sealed class CommandManager : IUpdateManager
{
    private readonly ITemplateService _templateService;
    private readonly ICurrentDialogNavigator _dialogNavigator;

    public CommandManager(ITemplateService templateService, ICurrentDialogNavigator dialogNavigator)
    {
        _templateService = templateService;
        _dialogNavigator = dialogNavigator;
    }

    private IDialogTemplate? _templateByCommand;

    /// <summary>
    /// Определяем, есть ли где-либо, где может храниться текст от пользователя отправленная команда
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public Task<bool> DoesItForMe(UpdateModelForManage model)
    {
        var falseResult = Task.FromResult(false);
        // if (_dialogNavigator.GetCurrentDialog().State == CurrentDialogState.BeingUsedByTemplate)
            return falseResult;
        // if (_dialogNavigator.GetCurrentDialog().CurrentStepType != null)
            return falseResult;
        
        _templateByCommand = _templateService.GetDialogTemplateByCommand(model.Update?.Message?.Text) ??
                             _templateService.GetDialogTemplateByCommand(model?.Update?.CallbackQuery?.Message?.Text);

        if (_templateByCommand == null)
            return falseResult;
        
        return Task.FromResult(true);
    }

    public async Task<bool> Manage(UpdateModelForManage model)
    {
        var foundTemplateByCommand = _templateByCommand!;
        var branchStep = foundTemplateByCommand.GetInitialBranchAndStepIds(model.Client);
        // Ищем первый шаг у найденного темплейта
        var intitialStep = foundTemplateByCommand.GetStepByStepAndBranchId(branchStep.Item2, branchStep.Item1, foundTemplateByCommand.GetType());
        if (intitialStep == null)
            throw new NullReferenceException($"Не удалось найти первый шаг у темплейта {foundTemplateByCommand.GetType()}");
        
        // Отправляем клиенту ответ в соответствии с правилами шага
        // await intitialStep.SendReplyToUser(_dialogNavigator.GetCurrentDialog(), foundTemplateByCommand.TranslationCollectionName);

        // _dialogNavigator.GetCurrentDialog().CurrentStepType = intitialStep.GetStepClassName();
        // if (_dialogNavigator.GetCurrentDialog().StepTypeHistory == null)
            // _dialogNavigator.GetCurrentDialog().StepTypeHistory = new List<string> { intitialStep.GetStepClassName() };
        // else
            // _dialogNavigator.GetCurrentDialog().StepTypeHistory.Add(intitialStep.GetStepClassName());
        
        // _dialogNavigator.GetCurrentDialog().State = CurrentDialogState.BeingUsedByTemplate;
        await _dialogNavigator.TryToSaveCurrentDialog();
        return true;
    }
}