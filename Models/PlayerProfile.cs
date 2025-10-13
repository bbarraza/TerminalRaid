using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TerminalRaid.Models
{
    public class PlayerProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProfileId { get; set; }

        [BsonElement("playerId")]
        public string PlayerId { get; set; }  // References PlayerCredentials.PlayerId

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("gameIpAddress")]
        public string GameIpAddress { get; set; }  // In-game IP for hacking mechanics

        [BsonElement("specs")]
        public PlayerSpecs Specs { get; set; }

        [BsonElement("walletAddress")]
        public string WalletAddress { get; set; }  // Reference to wallet

        [BsonElement("isOnline")]
        public bool IsOnline { get; set; }

        [BsonElement("lastSeen")]
        public DateTime LastSeen { get; set; }

        [BsonElement("joinedAt")]
        public DateTime JoinedAt { get; set; }

        public PlayerProfile()
        {
            ProfileId = ObjectId.GenerateNewId().ToString();
            Specs = new PlayerSpecs();
            IsOnline = false;
            JoinedAt = DateTime.UtcNow;
            LastSeen = DateTime.UtcNow;
        }

        public PlayerProfile(string playerId, string username) : this()
        {
            PlayerId = playerId;
            Username = username;
            GameIpAddress = GenerateGameIpAddress();
        }

        private string GenerateGameIpAddress()
        {
            var random = new Random();
            return $"192.168.{random.Next(1, 255)}.{random.Next(1, 255)}";
        }

        public void UpdateLastSeen()
        {
            LastSeen = DateTime.UtcNow;
        }

        public void SetOnlineStatus(bool online)
        {
            IsOnline = online;
            if (online)
                UpdateLastSeen();
        }
    }
}
