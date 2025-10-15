using MongoDB.Driver;
using TerminalRaid.Models;

namespace TerminalRaid.Services;
    public class AuthService
    {
        private readonly IMongoCollection<PlayerCredentials> _credentials;
        private readonly IMongoCollection<PlayerProfile> _profiles;
        private readonly IMongoCollection<RtkWallet> _wallets;

        public AuthService(MongoDbService MongoDbService)
        {
            _credentials = MongoDbService.Database.GetCollection<PlayerCredentials>("playerCredentials");
            _profiles = MongoDbService.Database.GetCollection<PlayerProfile>("playerProfiles");
            _wallets = MongoDbService.Database.GetCollection<RtkWallet>("wallets");
        }

        public async Task<PlayerCredentials> RegisterAsync(string username, string password)
        {
            // Check if username already exists
            var existing = await _credentials.Find(c => c.Username == username).FirstOrDefaultAsync();
            if (existing != null)
                throw new InvalidOperationException("Username already exists");

            // Create credentials first (generates PlayerId)
            var credentials = new PlayerCredentials(username, password);
            await _credentials.InsertOneAsync(credentials);

            // Create player profile
            var profile = new PlayerProfile(credentials.PlayerId, username);
            await _profiles.InsertOneAsync(profile);

            // Create wallet
            var wallet = new RtkWallet(username);
            profile.WalletAddress = wallet.Address;
            await _wallets.InsertOneAsync(wallet);
            
            // Update profile with wallet address
            await _profiles.ReplaceOneAsync(p => p.ProfileId == profile.ProfileId, profile);

            return credentials;
        }

        public async Task<PlayerCredentials?> LoginAsync(string username, string password)
        {
            var credentials = await _credentials.Find(c => c.Username == username).FirstOrDefaultAsync();
            if (credentials == null || !credentials.VerifyPassword(password))
                return null;

            // Update profile online status
            var profile = await _profiles.Find(p => p.PlayerId == credentials.PlayerId).FirstOrDefaultAsync();
            if (profile != null)
            {
                profile.SetOnlineStatus(true);
                await _profiles.ReplaceOneAsync(p => p.ProfileId == profile.ProfileId, profile);
            }

            return credentials;
        }

        public async Task LogoutAsync(string playerId)
        {
            var profile = await _profiles.Find(p => p.PlayerId == playerId).FirstOrDefaultAsync();
            if (profile != null)
            {
                profile.SetOnlineStatus(false);
                await _profiles.ReplaceOneAsync(p => p.ProfileId == profile.ProfileId, profile);
            }
        }
    }
