using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TerminalRaid.Models
{
    public class RtkTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("fromAddress")]
        public string FromAddress { get; set; }

        [BsonElement("toAddress")]
        public string ToAddress { get; set; }

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("transactionType")]
        public string TransactionType { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("transactionHash")]
        public string TransactionHash { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        public RtkTransaction()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Timestamp = DateTime.UtcNow;
            Status = "pending";
            TransactionHash = GenerateTransactionHash();
        }

        public RtkTransaction(string fromAddress, string toAddress, double amount, string transactionType, string description = "")
            : this()
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Amount = amount;
            TransactionType = transactionType;
            Description = description;
        }

        private string GenerateTransactionHash()
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var input = $"{FromAddress}_{ToAddress}_{Amount}_{Timestamp.Ticks}_{Guid.NewGuid()}";
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}