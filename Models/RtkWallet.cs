using System;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TerminalRaid.Models
{
public class RtkWallet
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    [BsonElement("ownerUsername")]
    public string OwnerUsername { get; set; }

    [BsonElement("address")]
    public string Address { get; set; }

    [BsonElement("WalletPassword")]
    public string WalletPassowrd {get; set;}

    [BsonElement("balance")]
    public double Balance { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    public List<RtkTransaction> Transactions { get; set; }

    public RtkWallet(string ownerUsername)
    {
        OwnerUsername = ownerUsername;
        Address = GenerateAddress(ownerUsername);
        Balance = 0.0;
        CreatedAt = DateTimeOffset.UtcNow;()
        Transactions = new List<RtkTransaction>();
    }

    private string GenerateAddress(string username)
    {
        using var sha256 = SHA256.Create();
        // Use a combination of username, current ticks, and a random GUID for uniqueness
        var uniqueInput = $"{username}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(uniqueInput));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower(); // 64 hex chars
    }
    private string GenerateWalletPassword(strng Password){
        using var sha256 = SHA256.Create();
        // Use a combination of password, current ticks, and a random GUID for uniqueness
        var uniqueInput = $"{Password}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(uniqueInput));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower(); // 64 hex chars
    }
}
}