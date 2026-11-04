
using UnityEngine;

public class Health : MonoBehaviour
{
    public int CurrentHealth { get; private set; } = 100;

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }
}
