using IT.Gateway;
using IT.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<KeyRotatorService>();

builder.Services.Configure<ServiceAddressesOptions>(
    builder.Configuration.GetSection(ServiceAddressesOptions.ConfigSectionName));

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

app.UseResponseCompression();
app.MapHealthChecks("/healthcheck");
app.MapControllers();

app.Run();