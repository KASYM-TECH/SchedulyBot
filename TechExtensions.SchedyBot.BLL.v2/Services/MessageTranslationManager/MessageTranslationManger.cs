using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;

namespace TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager
{

    public class MessageTranslationManger : IMessageTranslationManger
    {
        private readonly string _pathToDialogTextTranslation;
        private readonly ILogger<MessageTranslationManger> _logger;
        private readonly UpdateContainer _container;

        public MessageTranslationManger(
            IConfiguration configuration,
            ILogger<MessageTranslationManger> logger, 
            UpdateContainer container)
        {
            _logger = logger;
            _container = container;
            _pathToDialogTextTranslation = configuration["pathToDialogTextsTranslation"];
        }

        // TODO: Поработать над работой с json, можно сделать проще
        public async Task<string> GetTextByTag(string collectionName = "CommonTranslations", string texTag = "getBackBtn", string language = null)
        {
            await using var stream = new FileStream(
                _pathToDialogTextTranslation, 
                FileMode.Open,
                FileAccess.Read, 
                FileShare.Read);
            try
            {
                using var sr = new StreamReader(stream, Encoding.UTF8);

                var myJsonString = await sr.ReadToEndAsync();
                var translations = JObject.Parse(myJsonString).SelectToken("Translations");
                var neededCollection = translations
                    .Where(p => p.OfType<JProperty>()
                    .Any(j => j.Name == "CollectionName" && j.Value.ToString() == collectionName))
                    .ToList()
                    .First();

                var translatedWord = neededCollection[texTag][language != null ? language : _container.Client!.Language].ToString();
                return translatedWord;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Языковой код: {_container.Client!.Language}, название коллекции: {collectionName}, тег: {texTag}");
            }

            return null;
        }

        public async Task<string> GetTagByText(string collectionName, string text)
        {
            await using var stream = new FileStream(
                _pathToDialogTextTranslation, 
                FileMode.Open,
                FileAccess.Read, 
                FileShare.Read);
            
            try
            {
                using var sr = new StreamReader(stream, Encoding.UTF8);

                var myJsonString = await sr.ReadToEndAsync();
                var translations = JObject.Parse(myJsonString).SelectToken("Translations");
                var neededCollection = translations
                    .Where(p => p.OfType<JProperty>()
                    .Any(j => j.Name == "CollectionName" && j.Value.ToString() == collectionName))
                    .ToList().First();
                var neededTag = neededCollection
                    .OfType<JProperty>()
                    .FirstOrDefault(t =>
                        t.Value
                            .OfType<JProperty>()
                            .Any(s => s.Value.ToString() == text))?
                    .Name;

                return neededTag;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Языковой код: {_container.Client!.Language}, название коллекции: {collectionName}, текст: {text}");
            }

            return null;
        }
         
        public async Task<List<string>> GetAllTextsFromCollection(string collectionName)
        {
            await using var stream = new FileStream(
                _pathToDialogTextTranslation,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            try
            {
                using var sr = new StreamReader(stream, Encoding.UTF8);

                var myJsonString = await sr.ReadToEndAsync();
                var translations = JObject.Parse(myJsonString).SelectToken("Translations");
                var neededCollection = translations
                    .Where(p => p.OfType<JProperty>()
                    .Any(j => j.Name == "CollectionName" && j.Value.ToString() == collectionName))
                    .ToList().First()
                    .OfType<JProperty>()
                    .ToList().SkipWhile(l => l.Name == "templateName")
                    .ToList();

                var texts = new List<string>();
                foreach (var textBlock in neededCollection.OfType<JProperty>())
                    texts.Add(textBlock.Value.OfType<JProperty>().FirstOrDefault(g => g.Name == _container.Client!.Language)?.Value.ToString());
                
                return texts.Where(x => x != null).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Языковой код: {_container.Client!.Language}, название коллекции: {collectionName}");
            }

            return null;
        }
    }
}
