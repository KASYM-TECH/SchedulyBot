using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.DialogTemplates
{
    public class BaseDialogTemplate
    {
        private readonly IEnumerable<IDialogStep> _dialogSteps;
        public BaseDialogTemplate(IEnumerable<IDialogStep> dialogSteps)
        {
            _dialogSteps = dialogSteps;
        }

        /// <summary>
        /// верни степ по id степа и бранча
        /// </summary>
        /// <param name="stepId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IDialogStep? GetStepByStepAndBranchId(int stepId, int branchId, Type dialogTemplateType)
        {
            var foundStep = _dialogSteps
                .FirstOrDefault(s => s.DialogTemplateType == dialogTemplateType && s.DialogStepId == stepId && s.DialogBranchId == branchId);

            return foundStep;
        }

        /// <summary>
        /// Верни степ по строке
        /// </summary>
        public IDialogStep? GetStepByClassName(string stepName)
        {
            // TODO
            //var foundStep = _dialogSteps.FirstOrDefault(s => stepName.Contains(s.GetStepClassName()));
            return null;
        }
        public virtual Tuple<int, int> GetInitialBranchAndStepIds(UserDao userDao)
        {
            return new Tuple<int, int>(0, 0);
        }
        public bool IsThisStepLast(IDialogStep step)
        {
            // TODO
            var stepName = step.ToString();
            // var foundStep = _dialogSteps
            //     .FirstOrDefault(s => s.GetStepClassName() == stepName.ExtractDialogName());
            // var allIdsOfBranch = _dialogSteps
            //     .Where(s => s.DialogBranchId == foundStep.DialogBranchId)
            //     .Select(s => s.DialogStepId);
            // return ! allIdsOfBranch.Any(i => i > foundStep.DialogStepId);
            return false;
        }
    }
}