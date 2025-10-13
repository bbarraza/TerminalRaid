using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TerminalRaid.Services;
using TerminalRaid.Models;
using System.Threading.Tasks;

namespace TerminalRaid.Controllers;

[Route("api/[Controller]")]
[ApiController]

public class PlayerController : ControllerBase
{
        private readonly IMongoCollection<PlayerProfile> _profiles;
    
        public PlayerController(MongoDbService mongoDbService)
        { 
                _profiles = mongoDbService.Database?.GetCollection<PlayerProfile>("playerProfiles");
        }
    
        [HttpGet]
        public async Task<IEnumerable<PlayerProfile>> GetAllPlayers()
        {
                return await _profiles.Find(FilterDefinition<PlayerProfile>.Empty).ToListAsync();
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<PlayerProfile?>> GetPlayerByUsername(string username)
        {
                var profile = await _profiles.Find(p => p.Username == username).FirstOrDefaultAsync();
                return profile is not null ? Ok(profile) : NotFound();
        }

        [HttpGet("id/{playerId}")]
        public async Task<ActionResult<PlayerProfile?>> GetPlayerById(string playerId)
        {
                var profile = await _profiles.Find(p => p.PlayerId == playerId).FirstOrDefaultAsync();
                return profile is not null ? Ok(profile) : NotFound();
        }
        
        [HttpGet("online")]
        public async Task<IEnumerable<PlayerProfile>> GetOnlinePlayers()
        {
                return await _profiles.Find(p => p.IsOnline).ToListAsync();
        }

        [HttpGet("scan")]  // For network scanning mechanics
        public async Task<IEnumerable<object>> ScanNetwork()
        {
                var onlinePlayers = await _profiles.Find(p => p.IsOnline).ToListAsync();
                // Return only hackable information
                return onlinePlayers.Select(p => new {
                    Username = p.Username,
                    GameIpAddress = p.GameIpAddress,
                    Specs = p.Specs,
                    LastSeen = p.LastSeen
                });
        }
        


}