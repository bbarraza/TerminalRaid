using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using BCrypt.Net;

namespace TerminalRaid.Models
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PlayerId { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("ipAddress")]
        public string IpAddress { get; set; }

        [BsonElement("wallet")]
        public RtkWallet Wallet { get; set; }


        public Player(string username, string passwordHash, string ipAddress)
        {
            Username = username;
            PasswordHash = passwordHash;
            IpAddress = ipAddress;
            Wallet = new RtkWallet(username);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }

        public Player() { }
    }
}