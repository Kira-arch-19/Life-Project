
using NUnit.Framework;
using UnityEngine;

public class HealthTests
{
    [Test]
    public void TakeDamage_ReducesHealthBy20()
    {
        GameObject player = new GameObject("Player");
        Health health = player.AddComponent<Health>();

        try
        {
            health.TakeDamage(20);

            Assert.That(
                health.CurrentHealth,
                Is.EqualTo(80)
            );
        }
        finally
        {
            Object.DestroyImmediate(player);
        }
    }
}
