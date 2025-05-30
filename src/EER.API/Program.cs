using EER.API.Extensions;
using EER.Application.Extensions;
using EER.Infrastructure.Extensions;
using EER.Persistence.Migrations.Extensions;
using EER.Persistence.MongoDB.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters();

// Application
builder.Services.ConfigureMediatR();

// Persistence
builder.Services.ConfigureMigrationService();
//builder.Services.ConfigureDapper(builder.Configuration);
//builder.Services.ConfigureEntityFrameworkCore(builder.Configuration);
builder.Services.ConfigureMongo(builder.Configuration);

// Infrastructure
builder.Services.ConfigureSecurity();

// API
builder.Services.ConfigureCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerGen();

var app = builder.Build();

app.ConfigureExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsGlobalPolicy");

app.UseAuthorization();

app.MapControllers();

// app.ApplyMigrations(app.Services);
await app.Services.InitializeMongoDb();

app.Run();
