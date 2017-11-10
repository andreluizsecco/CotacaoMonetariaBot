using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CotacaoChatBot.Services
{
    public class TextAnalysis
    {
        private readonly ITextAnalyticsAPI client;

        public TextAnalysis()
        {
            client = new TextAnalyticsAPI();
            client.AzureRegion = AzureRegions.Eastus2;
            client.SubscriptionKey = "xxxxxxxxxxxxxxxxxxxxxxxx";
        }

        public async Task<string> GetMessageLanguageAsync(string message)
        {
            var language = string.Empty;

            LanguageBatchResult result = await client.DetectLanguageAsync(
                    new BatchInput(
                        new List<Input>()
                        {
                            new Input("1", message)
                        }
            ));

            if (result.Documents.Count > 0)
                language = result.Documents[0].DetectedLanguages[0].Iso6391Name;

            return language;
        }


        public async Task<double> GetSentimentScoreAsync(string language, string message)
        {
            double score = -1;

            SentimentBatchResult result = await client.SentimentAsync(
                    new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput(language, "1", message)
                        }));

            if (result.Documents.Count > 0)
                score = result.Documents[0].Score.Value;

            return score;
        }
    }
}