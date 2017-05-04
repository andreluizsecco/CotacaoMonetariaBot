using CotacaoChatBot.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CotacaoChatBot.Services
{
    [Serializable]
    public class CotacaoMoeda
    {
        public async Task<dynamic> Consultar(string url)
        {
            var client = new RestClient($"http://api.promasters.net.br/cotacao/v1/{url}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var result = response.Content;

            return JsonConvert.DeserializeObject(result);
        }

        public async Task<IEnumerable<Moeda>> Listagem()
        {
            var list = new List<Moeda>();
            var result = await Consultar("moedas?alt=json");

            foreach (var moeda in result.moedas)
            {
                var js = new DataContractJsonSerializer(typeof(Moeda));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(((Newtonsoft.Json.Linq.JContainer)moeda).First.ToString()));
                list.Add((Moeda)js.ReadObject(ms));
            }

            return list;
        }

        public async Task<IEnumerable<Cotacao>> Cotacao(string moeda)
        {
            var list = new List<Cotacao>();
            var siglaMoeda = string.Empty;

            if (!string.IsNullOrEmpty(moeda))
            {
                var currentScore = 0f;
                var listMoedas = await Listagem();
                listMoedas.ToList().ForEach(x =>
                {
                    var moedaScore = JaroWinklerDistance.GetDistance(x.nome, moeda);
                    if (moedaScore > currentScore)
                    {
                        currentScore = moedaScore;
                        siglaMoeda = x.moeda;
                    }
                });
            }

            var result = await Consultar($"valores?moedas={siglaMoeda}");

            foreach (var cotacao in result.valores)
            {
                var js = new DataContractJsonSerializer(typeof(Cotacao));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(((Newtonsoft.Json.Linq.JContainer)cotacao).First.ToString()));
                list.Add((Cotacao)js.ReadObject(ms));
            }

            return list;
        }
    }
}