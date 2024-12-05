using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Data;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure AppDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure()));

// Register services
builder.Services.AddScoped<InitDataService>();
builder.Services.AddScoped<EnvironmentalDataService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize data
using (var scope = app.Services.CreateScope())
{
    var initDataService = scope.ServiceProvider.GetRequiredService<InitDataService>();
    await initDataService.InitializeDataIfEmptyAsync();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();