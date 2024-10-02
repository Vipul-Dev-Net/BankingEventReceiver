using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingApi.EventReceiver
{
    public class Transaction
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("messageType")]
        public string MessageType { get; set; }

        [JsonPropertyName("bankAccountId")]
        public Guid BankAccountId { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; } 
    }
}
