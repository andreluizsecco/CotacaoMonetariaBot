using CotacaoChatBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CotacaoChatBot.Dialogs
{
    [LuisModel("{ModelID}", "{SubscriptionKey}")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        public RootDialog() { }

        [LuisIntent("conversation.hello")]
        public async Task Hello(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Olá! Sou o chatbot de Cotação de moedas, escreva o que deseja saber.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("conversation.about")]
        public async Task About(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sou um bot de Cotações, um assistente para sua consulta de cotações atuais de moedas estrangeiras! " +
                                    $"Fique a vontade para me dizer sobre qual moeda deseja buscar.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Desculpe! Não entendi o que deseja ou de que moeda está falando. " +
                                    $"Tente ser mais específico, por exemplo: \"Liste as moedas disponíveis\", \"Cotações do dia\" ou \"Cotação do Dolar\"");
            context.Wait(MessageReceived);
        }

        [LuisIntent("moeda.listagem")]
        public async Task MoedaListagem(IDialogContext context, LuisResult result)
        {
            var service = new CotacaoMoeda();
            var list = await service.Listagem();

            await context.PostAsync("Moedas disponíveis para consulta:");

            foreach(var item in list)
                await context.PostAsync($"{item.nome} ({item.moeda})");

            context.Wait(MessageReceived);
        }

        [LuisIntent("moeda.cotacao")]
        public async Task MoedaCotacao(IDialogContext context, LuisResult result)
        {
            var service = new CotacaoMoeda();
            var moeda = result.Entities.FirstOrDefault(x => x.Type.Equals("Moeda"));
            await context.PostAsync("Cotações:");
            
            var list = await service.Cotacao(moeda?.Entity ?? string.Empty);

            foreach(var cotacao in list)
            {
                var dataAtualizacao = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(cotacao.ultima_consulta).ToLocalTime();
                await context.PostAsync($"{cotacao.nome}: R$ {cotacao.valor.ToString("N4")}");
                await context.PostAsync($"Data: {dataAtualizacao} | Fonte: {cotacao.fonte}");
            }
            context.Wait(MessageReceived);
        }
    }
}