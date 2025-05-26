using EER.API.Extensions;
using EER.Application.Extensions;
using EER.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters();

builder.Services.ConfigureServices();
builder.Services.ConfigureMigrationService();
builder.Services.ConfigureCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerGen();

var app = builder.Build();

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

app.ApplyMigrations(app.Services);

app.Run();
