using System.Runtime.Serialization;

namespace CotacaoChatBot.Models
{
    [DataContract]
    public class Cotacao
    {
        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public float valor { get; set; }

        [DataMember]
        public int ultima_consulta { get; set; }

        [DataMember]
        public string fonte { get; set; }
    }
}