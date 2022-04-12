using System.Security.Cryptography;
using System.Text;
using IT.Encryption.Data;
using IT.Encryption.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IT.Encryption.Services;

public class AesEncryptionService : IEncryptionService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly EncryptionOptions _encryptionOptions;
    private readonly ILogger<AesEncryptionService> _logger;

    public AesEncryptionService(IConfiguration configuration,
        AppDbContext context,
        IOptions<EncryptionOptions> encryptionOptions,
        ILogger<AesEncryptionService> logger)
    {
        _configuration = configuration;
        _context = context;
        _encryptionOptions = encryptionOptions.Value;
        _logger = logger;
    }
    
    public async Task<EncryptionResult> EncryptData(string plainData)
    {
        var encKey = await _context.Keys.Where(k => k.Active && !k.Deleted)
            .OrderByDescending(k => k.CreatedAt)
            .FirstOrDefaultAsync();
        if (encKey is null)
        {
            // For the first time, there won't be any active keys. So, we need to Rotate to initiate process
            await RotateKey();
            encKey = await _context.Keys.Where(k => k.Active && !k.Deleted)
                .OrderByDescending(k => k.CreatedAt)
                .FirstAsync();
        }
        
        var tagBytes = Convert.FromBase64String(encKey.TagBase64);

        try
        {
            // var encryptedData = EncryptionHelpers.Encrypt(Encoding.UTF8.GetBytes(plainData), tagBytes,
            //     Encoding.ASCII.GetBytes(encKey.Key));
            var encryptedData = EncryptionHelpers.Encrypt(plainData, Encoding.ASCII.GetBytes(encKey.Key));
            // return EncryptionResult.Success(Convert.ToBase64String(await Task.FromResult(encryptedData)));
            return EncryptionResult.Success(encryptedData);
        }
        catch (CryptographicException cryptoEx)
        {
            _logger.LogError("Cryptography error occured: {ErrorMsg}", cryptoEx.Message);
            return EncryptionResult.Failed(ErrorReason.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError("Internal error occured: {ErrorMsg}", ex.Message);
            return EncryptionResult.Failed(ErrorReason.InternalServerError);
        }
    }

    public async Task<EncryptionResult> DecryptData(string encryptedData)
    {
        var cipherTextBytes = Encoding.UTF8.GetBytes(encryptedData);

        var keys = await _context.Keys.Where(k => !k.Deleted)
            .OrderByDescending(k => k.CreatedAt)
            .Take(10)
            .ToListAsync();
        foreach (var keyBytes in keys.Select(encKey => Encoding.ASCII.GetBytes(encKey.Key)))
        {
            try
            {
                // var plainDataBytes = EncryptionHelpers.Decrypt(cipherTextBytes, keyBytes);
                var plainDataBytes = EncryptionHelpers.Decrypt(encryptedData, keyBytes);
                // return EncryptionResult.Success(Encoding.ASCII.GetString(plainDataBytes));
                return EncryptionResult.Success(plainDataBytes);
            }
            catch (CryptographicException cryptoEx)
            {
                _logger.LogError("Cryptography error occured: {ErrorMsg}", cryptoEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal error occured: {ErrorMsg}", ex.Message);
                return EncryptionResult.Failed(ErrorReason.InternalServerError);
            }
        }

        return EncryptionResult.Failed(ErrorReason.InternalServerError);
    }

    public async Task<EncryptionResult> RotateKey()
    {
        string key;
        bool keyFound;
        do
        {
            key = RandomHelpers.RandomString(_encryptionOptions.KeyLength);
            keyFound = await _context.Keys.AnyAsync(k => k.Key == key);
        } while (keyFound);

        var tagBytes = Encoding.ASCII.GetBytes(RandomHelpers.RandomString(_encryptionOptions.TagLength));
        var newEncKey = new EncryptionKey
        {
            Key = key,
            TagBase64 = Convert.ToBase64String(tagBytes),
            Active = true
        };
        await _context.Keys.AddAsync(newEncKey);
        await _context.Keys.Where(k => k.Id != newEncKey.Id).ForEachAsync(k => k.Active = false);
        return await _context.SaveChangesAsync() == 0 
            ? EncryptionResult.Failed(ErrorReason.InternalServerError) 
            : EncryptionResult.Success();
    }
}