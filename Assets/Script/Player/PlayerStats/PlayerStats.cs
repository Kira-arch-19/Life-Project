using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField, Min(1)] private int baseHealth = 100;
    [SerializeField, Min(0)] private int attack = 20;
    [SerializeField, Min(0)] private int defense = 5;
    [SerializeField, Min(0f)] private float movementSpeed = 4f;

    public int BaseMaxHealth
    {
        get => baseHealth;
        set => baseHealth = Mathf.Max(1, value);
    }

    public int Attack
    {
        get => attack;
        set => attack = Mathf.Max(0, value);
    }

    public int Defense
    {
        get => defense;
        set => defense = Mathf.Max(0, value);
    }

    public float MovementSpeed
    {
        get => movementSpeed;
        set => movementSpeed = Mathf.Max(0f, value);
    }

    public Health Health { get; private set; }
    public Speed MvSpeed {get; private set;}
    public Attack AttackStats { get; private set; }
    public Defense DefenseStats { get; private set; }
    private void Awake()
    {
    Health = new Health(BaseMaxHealth);
    MvSpeed = new Speed(MovementSpeed);

    AttackStats = new Attack(Attack);
    DefenseStats = new Defense(Defense);
    }

}