namespace IT.Encryption.Models;

public class EncryptionKey : EntityAudit
{
    public string Key { get; set; } = null!;
    public string TagBase64 { get; set; } = null!;
    public bool Active { get; set; }
}