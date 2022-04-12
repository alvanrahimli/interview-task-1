using Flurl;
using Flurl.Http;
using IT.Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IT.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EncryptionController : ControllerBase
{
    private readonly ILogger<EncryptionController> _logger;
    private readonly ServiceAddressesOptions _options;
    
    public EncryptionController(IOptions<ServiceAddressesOptions> options, 
        ILogger<EncryptionController> logger)
    {
        _logger = logger;
        _options = options.Value;
    }
    
    [HttpPost("encrypt")]
    public async Task<ActionResult<DataModel>> GetEncryptedData(DataModel request)
    {
        try
        {
            var flurlResponse = await _options.EncryptionService.AppendPathSegments("api", "encryption", "encrypt")
                .PostJsonAsync(request);
            var responseJson = await flurlResponse.ResponseMessage.Content.ReadAsStringAsync();
            return Ok(JsonConvert.DeserializeObject<DataModel>(responseJson));
        }
        catch (FlurlHttpException e)
        {
            _logger.LogError("Could not get encrypted message: {ErrorMsg}", e.Message);
            return StatusCode(500, await e.GetResponseStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not get encrypted message: {ErrorMsg}", ex.Message);
            return StatusCode(500);
        }
    }
    
    [HttpPost("decrypt")]
    public async Task<ActionResult<DataModel>> GetDecryptedData(DataModel request)
    {
        try
        {
            var flurlResponse = await _options.EncryptionService.AppendPathSegments("api", "encryption", "decrypt")
                .PostJsonAsync(request);
            var responseJson = await flurlResponse.ResponseMessage.Content.ReadAsStringAsync();
            return Ok(JsonConvert.DeserializeObject<DataModel>(responseJson));
        }
        catch (FlurlHttpException e)
        {
            _logger.LogError("Could not get encrypted message: {ErrorMsg}", e.Message);
            return StatusCode(500, await e.GetResponseStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not get decrypted message: {ErrorMsg}", ex.Message);
            return StatusCode(500);
        }
    }
}