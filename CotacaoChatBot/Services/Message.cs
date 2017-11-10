using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CotacaoChatBot.Services
{
    public class Message
    {
        public static async Task Save(string message)
        {
            var textAnalyzer = new TextAnalysis();
            var language = await textAnalyzer.GetMessageLanguageAsync(message);
            var score = await textAnalyzer.GetSentimentScoreAsync(language, message);
            try
            {
                using (var cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    cnn.Open();
                    SqlParameter messageParameter = new SqlParameter("@Message", message);
                    SqlParameter languageParameter = new SqlParameter("@Language", language);
                    SqlParameter scoreParameter = new SqlParameter("@SentimentScore", score);
                    SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Messages (Message, Language, SentimentScore) VALUES (@Message, @Language, @SentimentScore)", cnn);
                    cmd.Parameters.Add(messageParameter);
                    cmd.Parameters.Add(languageParameter);
                    cmd.Parameters.Add(scoreParameter);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { }
        }
    }
}