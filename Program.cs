using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using TerminalRaid.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:6767");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<MiningService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(Options =>
    {
        Options.WithTitle("TerminalRaid API");
        Options.WithTheme(ScalarTheme.Purple);
        Options.WithLayout(ScalarLayout.Modern);
        
    });
}

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();