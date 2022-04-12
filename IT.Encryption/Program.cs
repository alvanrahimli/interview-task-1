using IT.Encryption;
using IT.Encryption.Data;
using IT.Encryption.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = new ServiceCollection();
services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
using (var serviceProvider = services.BuildServiceProvider())
{
    serviceProvider.MigrateDatabase();
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEncryptionService, AesEncryptionService>();

var app = builder.Build();
app.MapHealthChecks("/healthcheck");
app.MapControllers();

app.Run();