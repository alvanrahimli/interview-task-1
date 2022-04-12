namespace IT.Encryption.Models;

public class EntityAudit : EntityBase
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool Deleted { get; set; }
    public DateTime DeletedAt { get; set; }
    public Guid DeletedBy { get; set; }
}