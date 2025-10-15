namespace TerminalRaid.Services;

    public class FileService{
        private readonly ImongoCollection<WhichSystem> _CurrentIp;

        public FileService(MongoDbService MongoDbService){
            _CurrentIp = MongoDbService.Database.GetCollection<InsideSystem>("InsideSystem");
        }

        public async Task<Whichsystem> DefineSystemAsync(string GameIpAddress){
            var CurrentIp = _CurrentIp

        }
    }