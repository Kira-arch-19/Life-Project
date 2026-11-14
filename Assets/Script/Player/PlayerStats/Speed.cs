using UnityEngine;

public class Speed
{
    public float BaseSpeed { get; private set; }
    public float Multiplier { get; private set; } = 1f;

    public float CurrentSpeed => BaseSpeed * Multiplier;

    public Speed(float baseSpeed)
    {
        SetBaseSpeed(baseSpeed);
    }

    public void SetBaseSpeed(float speed)
    {
        BaseSpeed = Mathf.Max(0f, speed);
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