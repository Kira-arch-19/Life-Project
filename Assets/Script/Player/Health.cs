using NUnit.Framework;
using UnityEngine;

public class Health : MonoBehaviour
{
    int health = 100;
    void Start()
    {
        TakeDamage(20);
        Assert.That(health,Is.EqualTo(80));
    }
    private void TakeDamage(int Damage)
    {
        health-=Damage;
    }
}
