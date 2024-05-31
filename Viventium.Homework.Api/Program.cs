using Microsoft.EntityFrameworkCore;

using Viventium.Homework.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging((loggingBuilder)
    => loggingBuilder
        .SetMinimumLevel(LogLevel.Trace)
        .AddConsole()
        .AddDebug()
);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();

// Add db context
builder.Services.AddDbContext<CompaniesContext>(opt =>
       opt.UseInMemoryDatabase("Viventium"));

// Build and run...
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run("http://localhost:5010");
