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

// Configure HttpClient
builder.Services.AddHttpClient();

// Register services
builder.Services.AddScoped<InitDataService>();
builder.Services.AddScoped<EnvironmentalDataService>();
builder.Services.AddScoped<CoordinatesService>();
builder.Services.AddScoped<ConvertDt>();
builder.Services.AddScoped<CrawDataService>();
builder.Services.AddHostedService<ScheduledDataCrawlService>();

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Initialize data
using (var scope = app.Services.CreateScope())
{
    var initDataService = scope.ServiceProvider.GetRequiredService<InitDataService>();
    await initDataService.InitializeDataIfEmptyAsync();
}

app.Run();