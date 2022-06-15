using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions
{
    /// <summary>
    /// До попадания на любой из этих шагов, User должен быть уже в БД с заполненным ChatId  
    /// </summary>
    public interface IDialogStep
    {
        public bool LastStep { get; set; }

        /// <summary>
        /// Отвечает юсеру
        /// </summary>
        /// <param name="currentDialog"></param>
        /// <returns></returns>
        public Task SendReplyToUser();

        /// <summary>
        /// Обрабатывает ответ клиента и Возвращает следующую итерацию диалога, если такая есть
        /// </summary>
        /// <param name="clientAnswer"></param>
        /// <returns></returns>
        public Task<DialogIteration> HandleAnswerAndGetNextIteration(string clientAnswer);
        
        public int TemplateId { get; }
        public int BranchId { get; }
        public int StepId { get; }
    }
}
