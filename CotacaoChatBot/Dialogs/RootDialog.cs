using CotacaoChatBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
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
            var message = string.Empty;
            var service = new CotacaoMoeda();
            try
            {
                var list = await service.Listagem();
                var cardActions = new List<CardAction>();

                cardActions.Add(new CardAction()
                {
                    Title = $"Todas as cotações",
                    Type = ActionTypes.ImBack,
                    Value = $"Cotações do dia"
                });

                foreach (var item in list)
                    cardActions.Add(new CardAction()
                    {
                        Title = $"{item.nome} ({item.moeda})",
                        Type = ActionTypes.ImBack,
                        Value = $"Cotação {item.nome}"
                    });

                var card = new HeroCard()
                {
                    Title = "Moedas disponíveis",
                    Subtitle = "Clique na moeda que deseja consultar",
                    Buttons = cardActions
                };

                var activity = context.MakeMessage();
                activity.Id = new Random().Next().ToString();
                activity.Attachments.Add(card.ToAttachment());
                
                await context.PostAsync(activity);
            }
            catch(Exception ex)
            {
                message = "Desculpe, não consegui buscar essa informação no momento. Se importa de tentar novamente?";
                await context.PostAsync(message);
            }
            
            context.Wait(MessageReceived);
        }

        [LuisIntent("moeda.cotacao")]
        public async Task MoedaCotacao(IDialogContext context, LuisResult result)
        {
            var message = string.Empty;
            var service = new CotacaoMoeda();
            try
            {
                var moeda = result.Entities.FirstOrDefault(x => x.Type.Equals("Moeda"));
                message = "Cotações:\n\n";

                var list = await service.Cotacao(moeda?.Entity ?? string.Empty);

                foreach (var cotacao in list)
                {
                    var dataAtualizacao = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(cotacao.ultima_consulta), TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
                    message += $"**{cotacao.nome}**: R$ {cotacao.valor.ToString("N4")}\n\n";
                    message += $"Data: {dataAtualizacao.ToString("dd/MM/yyyy HH:mm:ss")} | Fonte: {cotacao.fonte}\n\n";
                }
            }
            catch (Exception ex)
            {
                message = "Desculpe, não consegui buscar essa informação no momento. Se importa de tentar novamente?";
            }
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}