namespace ExamenPOO.Core.Entities;

public abstract class BaseEntity
{
    public int Id { get; internal set; }
    public DateTime CreatedAt { get; internal set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; internal set; }

    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
