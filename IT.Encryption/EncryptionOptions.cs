namespace IT.Encryption;

public class EncryptionOptions
{
    public const string ConfigSectionName = "EncryptionOptions";

    public int KeyLength = 16;
    public int TagLength = 12;
}