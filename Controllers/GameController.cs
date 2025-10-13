using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TerminalRaid.Models;
using TerminalRaid.Services;
using System.Threading.Tasks;

namespace TerminalRaid.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("state")]
    public async Task<ActionResult<GameState>> GetGameState()
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        var state = await _gameService.GetGameStateAsync(username);
        return Ok(state);
    }

    // Move player to a new position
    [HttpPost("move")]
    public async Task<ActionResult<GameState>> Move([FromBody] MoveRequest move)
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        var state = await _gameService.MovePlayerAsync(username, move.Direction);
        return Ok(state);
    }

    // Attempt to hack another player
    [HttpPost("hack")]
    public async Task<ActionResult<HackingAttemptResult>> Hack([FromBody] HackRequest hack)
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        var result = await _gameService.AttemptHackAsync(username, hack.TargetUsername, hack.Method);
        return Ok(result);
    }

    // Collect offline resources
    [HttpPost("collect")]
    public async Task<ActionResult<ResourceInfo>> CollectResources()
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        var resources = await _gameService.CollectResourcesAsync(username);
        return Ok(resources);
    }
}

// Example request/response models
public class MoveRequest
{
    public string Direction { get; set; } // e.g., "north", "south"
}

public class HackRequest
{
    public string TargetUsername { get; set; }
    public string Method { get; set; } // e.g., "bruteforce", "phishing"
}

public class HackingAttemptResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int ResourceStolen { get; set; }
}

public class ResourceInfo
{
    public int ResourcesCollected { get; set; }
    public string ResourceType { get; set; }
}

public class GameState
{
    public string Username { get; set; }
    public string Position { get; set; }
    public int Resources { get; set; }
    public string Status { get; set; } // e.g., "safe", "under attack"
    // Add more fields as needed!
}