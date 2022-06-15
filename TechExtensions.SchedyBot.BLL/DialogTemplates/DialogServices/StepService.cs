namespace TechExtensions.SchedyBot.BLL.DialogTemplates.DialogServices
{
    public static class StepService
    {
        private static string _getBackBtnTag = TextTag.GetBackBtn;
        private static string _cancelBtnTag = TextTag.CancelBtn;
        public static async Task<List<string>> AddBaseButtons(IMessageTranslationManger messageTranslationManager)
        {
            var getBackBtnTag = await messageTranslationManager.GetTranslatedTextByTag(CollectionName.CommonTranslations, _getBackBtnTag);
            var cancelBtnTag = await messageTranslationManager.GetTranslatedTextByTag(CollectionName.CommonTranslations, _cancelBtnTag);

            var listWithBtns = new List<string>() { getBackBtnTag, cancelBtnTag };
            return listWithBtns;
        }


    }
}
