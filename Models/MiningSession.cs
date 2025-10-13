using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TerminalRaid.Models;
    public class MiningSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public double RTKPerSecond { get; set; }
        public DateTimeOffset StartTimestamp { get; set; }
        public DateTimeOffset EndTimestamp { get; set; }
        public double TotalRTKProduced { get; set; }

        public MiningSession(string playerId, double rtkPerSecond)
        {
            Id = ObjectId.GenerateNewId().ToString();
            OwnerId = playerId;
            RTKPerSecond = rtkPerSecond;
            StartTimestamp = DateTimeOffset.UtcNow;
            EndTimestamp = DateTimeOffset.UtcNow.AddYears(1); // Set far future for active sessions
            TotalRTKProduced = 0;
        }

        public double CalculatePendingRtk()
        {
            if (!IsActive()) return 0;
            
            var now = DateTimeOffset.UtcNow;
            var miningDuration = (now - StartTimestamp).TotalSeconds;
            return miningDuration * RTKPerSecond;
        }

        public bool IsActive()
        {
            return EndTimestamp > DateTimeOffset.UtcNow;
        }

        public void StopMining()
        {
            EndTimestamp = DateTimeOffset.UtcNow;
        }

        public TimeSpan GetMiningDuration()
        {
            var endTime = IsActive() ? DateTimeOffset.UtcNow : EndTimestamp;
            return endTime - StartTimestamp;
        }
    }