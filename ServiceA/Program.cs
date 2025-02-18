using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// registering health check services
builder.Services.AddHealthChecks()
    .AddSqlServer("Server=127.0.0.1,1433; Database=ServiceA; User Id=sa; Password=SQLServer1!; TrustServerCertificate=True")
    .AddRedis("localhost");

builder.Services.AddDbContext<ApplicationDbContext>(
        options => options.UseSqlServer("Server=127.0.0.1,1433; Database=ServiceA; User Id=sa; Password=SQLServer1!; TrustServerCertificate=True"));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// The "/health" endpoint will return a status regarding the ServiceA  
app.MapHealthChecks("/health",
 new HealthCheckOptions
 {
     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
 });

app.MapControllers();

app.Run();

internal class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}