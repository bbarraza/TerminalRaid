using MongoDB.Driver;
using TerminalRaid.Models;

namespace TerminalRaid.Services;
    public class MiningService
    {
        private readonly IMongoCollection<MiningSession> _miningSessions;
        private readonly IMongoCollection<PlayerProfile> _playerProfiles;
        private readonly IMongoCollection<RtkWallet> _wallets;

        public MiningService(MongoDbService MongoDbService)
        {
            _miningSessions = MongoDbService.Database?.GetCollection<MiningSession>("miningSessions");
            _playerProfiles = MongoDbService.Database?.GetCollection<PlayerProfile>("playerProfiles");
            _wallets = MongoDbService.Database?.GetCollection<RtkWallet>("wallets");
        }

        public async Task<MiningSession?> StartMiningAsync(string playerId)
        {
            // Get player profile to calculate hashrate
            var profile = await _playerProfiles.Find(p => p.PlayerId == playerId).FirstOrDefaultAsync();
            if (profile == null) return null;

            // Stop any existing active sessions
            await StopMiningAsync(playerId);

            // Calculate RTK per second based on hardware specs
            var rtkPerSecond = CalculateHashrate(profile.Specs);
            
            var session = new MiningSession(playerId, rtkPerSecond);
            await _miningSessions.InsertOneAsync(session);
            
            return session;
        }

        public async Task<double> ClaimRewardsAsync(string playerId)
        {
            var session = await GetActiveMiningSessionAsync(playerId);
            if (session == null) return 0;

            var pendingRtk = CalculatePendingRtk(session);
            if (pendingRtk <= 0) return 0;

            // Get player's wallet
            var profile = await _playerProfiles.Find(p => p.PlayerId == playerId).FirstOrDefaultAsync();
            if (profile == null) return 0;

            var wallet = await _wallets.Find(w => w.Address == profile.WalletAddress).FirstOrDefaultAsync();
            if (wallet == null) return 0;

            // Update wallet balance
            wallet.Balance += pendingRtk;
            
            // Create transaction record
            var transaction = new RtkTransaction(
                "mining_pool", 
                wallet.Address, 
                pendingRtk, 
                "mining_reward", 
                $"AFK Mining reward: {pendingRtk:F4} RTK"
            );
            transaction.Status = "confirmed";
            wallet.Transactions.Add(transaction);

            // Update session
            session.StopMining();
            session.TotalRTKProduced += pendingRtk;

            // Save changes
            await _wallets.ReplaceOneAsync(w => w.Address == wallet.Address, wallet);
            await _miningSessions.ReplaceOneAsync(s => s.Id == session.Id, session);

            return pendingRtk;
        }

        public async Task<bool> StopMiningAsync(string playerId)
        {
            var session = await GetActiveMiningSessionAsync(playerId);
            if (session == null) return false;

            session.StopMining();
            await _miningSessions.ReplaceOneAsync(s => s.Id == session.Id, session);
            return true;
        }

        public async Task<MiningSession?> GetActiveMiningSessionAsync(string playerId)
        {
            return await _miningSessions
                .Find(s => s.OwnerId == playerId && s.EndTimestamp > DateTimeOffset.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task<double> GetPendingRewardsAsync(string playerId)
        {
            var session = await GetActiveMiningSessionAsync(playerId);
            return session != null ? CalculatePendingRtk(session) : 0;
        }

        private double CalculatePendingRtk(MiningSession session)
        {
            return session.CalculatePendingRtk();
        }

        private double CalculateHashrate(PlayerSpecs specs)
        {
            var cpuPower = specs.CpuTier * 0.0001;      // CPU: 0.0001 RTK/sec per tier
            var gpuPower = specs.GpuTier * 0.0003;      // GPU: 0.0003 RTK/sec per tier 
            var ramBonus = specs.RamTier * 0.00005;     // RAM: 0.00005 RTK/sec per tier
            var psuEfficiency = specs.PsuTier * 0.00002; // PSU: 0.00002 RTK/sec per tier
            
            var baseHashrate = cpuPower + gpuPower + ramBonus + psuEfficiency;
            
            var random = new Random();
            var variance = (random.NextDouble() - 0.5) * 0.2;
            
            return Math.Max(0.00001, baseHashrate * (1 + variance));
        }

        public async Task<List<MiningSession>> GetMiningHistoryAsync(string playerId, int limit = 10)
        {
            return await _miningSessions
                .Find(s => s.OwnerId == playerId)
                .SortByDescending(s => s.StartTimestamp)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<object> GetMiningStatsAsync(string playerId)
        {
            var sessions = await _miningSessions.Find(s => s.OwnerId == playerId).ToListAsync();
            
            return new {
                TotalSessions = sessions.Count,
                TotalRTKEarned = sessions.Sum(s => s.TotalRTKProduced),
                TotalMiningTime = sessions.Sum(s => (s.EndTimestamp - s.StartTimestamp).TotalHours),
                AverageRTKPerHour = sessions.Any() ? sessions.Average(s => s.RTKPerSecond * 3600) : 0
            };
        }
    }