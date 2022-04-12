using IT.Gateway;
using IT.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<KeyRotatorService>();

builder.Services.Configure<ServiceAddressesOptions>(
    builder.Configuration.GetSection(ServiceAddressesOptions.ConfigSectionName));

var app = builder.Build();

app.MapHealthChecks("/healthcheck");
app.MapControllers();

app.Run();