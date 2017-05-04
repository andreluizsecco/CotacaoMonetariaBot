using System.Runtime.Serialization;

namespace CotacaoChatBot.Models
{
    [DataContract]
    public class Moeda
    {
        [DataMember]
        public string moeda { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public string fonte { get; set; }
    }
}