namespace IT.Encryption.Models;

public class EntityBase
{
    public Guid Id { get; init; }

    public override string ToString()
    {
        return $"{GetType().Name}[Id={Id}]";
    }
}