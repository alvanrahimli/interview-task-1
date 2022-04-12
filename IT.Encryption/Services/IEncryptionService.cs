namespace IT.Encryption.Services;

public interface IEncryptionService
{
    Task<EncryptionResult> EncryptData(string plainData);
    Task<EncryptionResult> DecryptData(string encryptedData);
    Task<EncryptionResult> RotateKey();
}