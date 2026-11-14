using UnityEngine;

public class Attack
{
    public int BaseDamage { get; private set; }
    public float Multiplier { get; private set; } = 1f;

    public int CurrentDamage =>
        Mathf.RoundToInt(BaseDamage * Multiplier);

    public Attack(int baseDamage)
    {
        SetBaseDamage(baseDamage);
    }

    public void SetBaseDamage(int damage)
    {
        BaseDamage = Mathf.Max(0, damage);
    }

    public void SetMultiplier(float multiplier)
    {
        Multiplier = Mathf.Max(0f, multiplier);
    }

    public void ResetMultiplier()
    {
        Multiplier = 1f;
    }
}