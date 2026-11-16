
namespace LifeProject.Api.Models;

public class House
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PlayerId { get; set; }

    public Player Player { get; set; } = null!;

    // Stato iniziale della mansarda: 0 = da restaurare.
    public int AtticRestorationStage { get; set; } = 0;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
