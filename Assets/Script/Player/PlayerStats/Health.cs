using UnityEngine;

public class Health
{
    public int CurrentHealth { get; private set; }
    private int MaxHealth { get; set; }

    public Health(int baseMaxHealth)
    {
        MaxHealth = Mathf.Max(1, baseMaxHealth);
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead())
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
    }

    public bool IsDead() => CurrentHealth == 0;

    public void Heal(int healAmount)
    {
        if (healAmount <= 0 || IsDead())
            return;

        CurrentHealth += Mathf.Min(healAmount, MaxHealth - CurrentHealth);
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }

    public void DecreaseMaxHealth(int amount)
    {
        if (amount <= 0)
            return;

        MaxHealth = Mathf.Max(1, MaxHealth - amount);
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (amount <= 0)
            return;

        MaxHealth += Mathf.Min(amount, int.MaxValue - MaxHealth);
    }

    public int GetMaxHealth() => MaxHealth;
}