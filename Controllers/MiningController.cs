using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TerminalRaid.Services;
using TerminalRaid.Models;

namespace TerminalRaid.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MiningController : ControllerBase
{
    private readonly MiningService _miningService;
    private readonly IMongoCollection<PlayerCredentials> _credentials;

    public MiningController(MiningService miningService, MongoDbService mongoDbService)
    {
        _miningService = miningService;
        _credentials = mongoDbService.Database?.GetCollection<PlayerCredentials>("playerCredentials");
    }

    [HttpPost("start")]
    public async Task<ActionResult> StartMining()
    {
        var username = User.Identity!.Name!;
        var credentials = await _credentials.Find(c => c.Username == username).FirstOrDefaultAsync();
        if (credentials == null) return NotFound("Player not found");

        var session = await _miningService.StartMiningAsync(credentials.PlayerId);
        if (session == null) return BadRequest("Could not start mining session");

        return Ok(new { 
            message = "Mining started successfully",
            sessionId = session.Id,
            rtkPerSecond = session.RTKPerSecond
        });
    }

    [HttpPost("stop")]
    public async Task<ActionResult> StopMining()
    {
        var username = User.Identity!.Name!;
        var credentials = await _credentials.Find(c => c.Username == username).FirstOrDefaultAsync();
        if (credentials == null) return NotFound("Player not found");

        var stopped = await _miningService.StopMiningAsync(credentials.PlayerId);
        if (!stopped) return BadRequest("No active mining session found");

        return Ok(new { message = "Mining stopped successfully" });
    }

    [HttpPost("claim")]
    public async Task<ActionResult> ClaimRewards()
    {
        var username = User.Identity!.Name!;
        var credentials = await _credentials.Find(c => c.Username == username).FirstOrDefaultAsync();
        if (credentials == null) return NotFound("Player not found");

        var rewards = await _miningService.ClaimRewardsAsync(credentials.PlayerId);
        if (rewards <= 0) return BadRequest("No rewards to claim");

        return Ok(new { 
            message = "Rewards claimed successfully",
            rtkEarned = rewards
        });
    }

    [HttpGet("status")]
    public async Task<ActionResult> GetMiningStatus()
    {
        var username = User.Identity!.Name!;
        var credentials = await _credentials.Find(c => c.Username == username).FirstOrDefaultAsync();
        if (credentials == null) return NotFound("Player not found");

        var session = await _miningService.GetActiveMiningSessionAsync(credentials.PlayerId);
        var pendingRewards = await _miningService.GetPendingRewardsAsync(credentials.PlayerId);

        return Ok(new {
            isActive = session != null,
            sessionId = session?.Id,
            rtkPerSecond = session?.RTKPerSecond ?? 0,
            pendingRewards = pendingRewards,
            startTime = session?.StartTimestamp,
            duration = session?.GetMiningDuration().TotalMinutes ?? 0
        });
    }

    [HttpGet("history")]
    public async Task<ActionResult> GetMiningHistory([FromQuery] int limit = 10)
    {
        var username = User.Identity!.Name!;
        var credentials = await _credentials.Find(c => c.Username == username).FirstOrDefaultAsync();
        if (credentials == null) return NotFound("Player not found");

        var history = await _miningService.GetMiningHistoryAsync(credentials.PlayerId, limit);
        return Ok(history);
    }

    [HttpGet("stats")]
    public async Task<ActionResult> GetMiningStats()
    {
        var username = User.Identity!.Name!;
        var credentials = await _credentials.Find(c => c.Username == username).FirstOrDefaultAsync();
        if (credentials == null) return NotFound("Player not found");

        var stats = await _miningService.GetMiningStatsAsync(credentials.PlayerId);
        return Ok(stats);
    }
}
