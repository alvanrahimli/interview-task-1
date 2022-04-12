using IT.Encryption.Dtos;
using IT.Encryption.Services;
using Microsoft.AspNetCore.Mvc;

namespace IT.Encryption.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EncryptionController : ControllerBase
{
    private readonly IEncryptionService _encryptionService;

    public EncryptionController(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    [HttpPost("encrypt")]
    public async Task<ActionResult<DataModel>> EncryptData(DataModel request)
    {
        var encryptionResult = await _encryptionService.EncryptData(request.Data);
        if (encryptionResult.Succeeded && encryptionResult.Data is not null)
        {
            return Ok(new DataModel((string) encryptionResult.Data));
        }

        return StatusCode(encryptionResult.ErrorReason switch
        {
            ErrorReason.KeyNotFound => 403,
            _ => 500
        });
    }
    
    [HttpPost("decrypt")]
    public async Task<ActionResult<DataModel>> DecryptData(DataModel request)
    {
        var decryptionResult = await _encryptionService.DecryptData(request.Data);
        if (decryptionResult.Succeeded && decryptionResult.Data is not null)
        {
            return Ok(new DataModel((string) decryptionResult.Data));
        }

        return StatusCode(decryptionResult.ErrorReason switch
        {
            ErrorReason.KeyNotFound => 403,
            _ => 500
        });
    }
    
    [HttpPost("rotate-key")]
    public async Task<ActionResult> RotateKey()
    {
        var rotationResult = await _encryptionService.RotateKey();
        return rotationResult.Succeeded
            ? Ok()
            : StatusCode(500);
    }
}