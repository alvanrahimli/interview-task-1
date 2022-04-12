using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace IT.Gateway.Services;

public class KeyRotatorService : BackgroundService
{
    private readonly ILogger<KeyRotatorService> _logger;
    private readonly ServiceAddressesOptions _serviceAddresses;

    public KeyRotatorService(IOptions<ServiceAddressesOptions> serviceAddresses,
        ILogger<KeyRotatorService> logger)
    {
        _logger = logger;
        _serviceAddresses = serviceAddresses.Value;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _serviceAddresses.EncryptionService
                    .AppendPathSegment("api/encryption/rotate-key")
                    .PostAsync(cancellationToken: stoppingToken);
                _logger.LogInformation("Key rotation succeeded");
            }
            catch (FlurlHttpException flurlEx)
            {
                _logger.LogError("Could not rotate key: {ErrorMsg}", flurlEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal error occured: {ErrorMsg}", ex.Message);
            }
            finally
            {
                Thread.Sleep(60 * 1000); // 60 seconds
            }
        }
    }
}