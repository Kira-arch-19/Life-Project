using NUnit.Framework;
using UnityEngine;

public class HealthTests
{
    private GameObject player;
    private Health health;

    [SetUp]
    public void SetUp()
    {
        player = new GameObject("Player");
        health = player.AddComponent<Health>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
    }

    [Test]
    public void TakeDamage_ReducesHealthBy20()
    {
        health.TakeDamage(20);

        Assert.That(health.CurrentHealth, Is.EqualTo(80));
    }

    [Test]
    public void DecreaseMaxHealth_ClampsCurrentHealth()
    {
        health.DecreaseMaxHealth(10);

        Assert.That(health.GetMaxHealth(), Is.EqualTo(90));
        Assert.That(health.CurrentHealth, Is.EqualTo(90));
    }

    [Test]
    public void ResetHealth_RestoresCurrentMaximum()
    {
        health.DecreaseMaxHealth(10);
        health.TakeDamage(30);

        health.ResetHealth();

        Assert.That(health.CurrentHealth, Is.EqualTo(90));
    }

    [Test]
    public void IncreaseMaxHealth_DoesNotHeal()
    {
        health.IncreaseMaxHealth(10);

        Assert.That(health.GetMaxHealth(), Is.EqualTo(110));
        Assert.That(health.CurrentHealth, Is.EqualTo(100));
    }

    [Test]
    public void Heal_DoesNotExceedMaximum()
    {
        health.TakeDamage(20);

        health.Heal(50);

        Assert.That(health.CurrentHealth, Is.EqualTo(100));
    }

    [Test]
    public void Heal_RestoresRequestedAmount()
    {
        health.TakeDamage(40);

        health.Heal(10);

        Assert.That(health.CurrentHealth, Is.EqualTo(70));
    }
}