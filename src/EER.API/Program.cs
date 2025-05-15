var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

DateTime dateTime = DateTime.Now;
DateTime dateTim = DateTime.Now;

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();