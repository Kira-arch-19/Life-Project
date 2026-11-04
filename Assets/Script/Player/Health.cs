
using UnityEngine;

public class Health : MonoBehaviour
{
    public int CurrentHealth { get; private set; } = 100;
    private int MaxHealth{ get; set; } =  100;
    public void TakeDamage(int damage)
    {
        if(damage <= 0 || IsDead())
            return;
        CurrentHealth = Mathf.Max(CurrentHealth -= damage, 0);
    }
    public bool  IsDead()
    {
        return CurrentHealth <= 0;
    }// verifica se sei morto o meno
    public void Heal(int healAmount)
    {
        if(healAmount <= 0 || IsDead())
            return; 
       CurrentHealth = Mathf.Min(CurrentHealth += healAmount, 100);
    }// healing
    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    } //reset health 
    public void DecreaseMaxHealth(int amount)
    {
        MaxHealth = Mathf.Max(MaxHealth - amount, 1);
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
    } // decrease max health
    public void IncreaseMaxHealth(int amount)
    {
        MaxHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
    }    // increase max health
    public int GetMaxHealth()
    {
        return MaxHealth;
    } // get max health
}

