namespace IT.Encryption;

public class EncryptionResult
{
    public bool Succeeded { get; set; }
    public ErrorReason? ErrorReason { get; set; }
    public object? Data { get; set; }
    
    private EncryptionResult()
    { }

    public static EncryptionResult Success(object? data = null)
    {
        return new EncryptionResult
        {
            Succeeded = true,
            Data = data
        };
    }
    
    public static EncryptionResult Failed(ErrorReason reason, object? data = null)
    {
        return new EncryptionResult
        {
            Succeeded = false,
            Data = data,
            ErrorReason = reason
        };
    }
}

public enum ErrorReason
{
    KeyNotFound, InternalServerError
}