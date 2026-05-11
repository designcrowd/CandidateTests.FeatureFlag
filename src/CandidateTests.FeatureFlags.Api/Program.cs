using CandidateTests.FeatureFlags.Api.Data;
using CandidateTests.FeatureFlags.Api.Evaluation;
using CandidateTests.FeatureFlags.Api.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFlagDao>(sp =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Default")
                           ?? "Data Source=flags.db";
    return new FlagDao(connectionString);
});
builder.Services.AddSingleton<FlagEvaluator>();
builder.Services.AddSingleton<IDbMigratorService>(sp =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Default")
                           ?? "Data Source=flags.db";
    var logger = sp.GetRequiredService<ILogger<DbMigratorService>>();
    return new DbMigratorService(connectionString, logger);
});

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:4000")
     .AllowAnyHeader()
     .AllowAnyMethod()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Services.GetRequiredService<IDbMigratorService>().MigrateDb();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program { }
