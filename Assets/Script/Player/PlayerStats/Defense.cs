using UnityEngine;

public class Defense
{
    public int BaseDefense { get; private set; }
    public float Multiplier { get; private set; } = 1f;

    public int CurrentDefense =>
        Mathf.RoundToInt(BaseDefense * Multiplier);

    public Defense(int baseDefense)
    {
        SetBaseDefense(baseDefense);
    }

    public void SetBaseDefense(int defense)
    {
        BaseDefense = Mathf.Max(0, defense);
    }

    public void SetMultiplier(float multiplier)
    {
        Multiplier = Mathf.Max(0f, multiplier);
    }

    public void ResetMultiplier()
    {
        Multiplier = 1f;
    }

    public int ReduceDamage(int incomingDamage)
    {
        incomingDamage = Mathf.Max(0, incomingDamage);

        return Mathf.Max(0, incomingDamage - CurrentDefense);
    }
}