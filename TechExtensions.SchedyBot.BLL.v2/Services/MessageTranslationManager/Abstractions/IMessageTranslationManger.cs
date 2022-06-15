namespace TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions
{
    /// <summary>
    /// Достает переводы из файла xml, (алгортм протестировал в конольном приложении, работает)
    /// </summary>
    public interface IMessageTranslationManger
    {
        /// <summary>
        /// Возвращает переведенный текст по тэгу
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="pathToDialogTemplate"></param>
        /// <param name="wordTag"></param>
        /// <returns></returns>
        public Task<string> GetTextByTag(string collectionName, string texTag, string language = null);

        /// <summary>
        /// Возвращает по тэгу переведенный текст
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="pathToDialogTemplate"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<string> GetTagByText(string collectionName, string text);

        /// <summary>
        /// Возвращает все тексты из коллекци кроме collectionName
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public Task<List<string>> GetAllTextsFromCollection(string collectionName);
    }
}
