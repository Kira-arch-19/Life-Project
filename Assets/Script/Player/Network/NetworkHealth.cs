using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(NetworkObject))]
public class NetworkHealth : NetworkBehaviour
{
    private PlayerStats playerStats;

    // Tutti possono leggere i valori.
    // Solo il server può modificarli.
    private readonly NetworkVariable<int> currentHealth = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private readonly NetworkVariable<int> maxHealth = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // La UI userà queste proprietà per leggere la vita sincronizzata.
    public int CurrentHealth => currentHealth.Value;
    public int MaxHealth => maxHealth.Value;

    public bool IsDead => IsSpawned && CurrentHealth <= 0;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    public override void OnNetworkSpawn()
    {
        // Alla creazione in rete, il server pubblica i valori iniziali.
        if (IsServer)
        {
            SynchronizeHealth();
        }
    }

    private void LateUpdate()
    {
        if (!IsSpawned || !IsServer)
            return;

        // Rileva anche eventuali modifiche fatte da altri sistemi
        // alla Health del server.
        SynchronizeHealth();
    }

    public void TakeDamage(int amount)
    {
        // Una chiamata locale sul client non applica danni.
        if (!IsSpawned || !IsServer)
            return;

        // Le regole del danno rimangono nella classe Health.
        playerStats.Health.TakeDamage(amount);

        SynchronizeHealth();
    }

    public void Heal(int amount)
    {
        if (!IsSpawned || !IsServer)
            return;

        playerStats.Health.Heal(amount);

        SynchronizeHealth();
    }

    public void ResetHealth()
    {
        if (!IsSpawned || !IsServer)
            return;

        playerStats.Health.ResetHealth();

        SynchronizeHealth();
    }

    private void SynchronizeHealth()
    {
        // Non crea una nuova Health:
        // usa quella già posseduta da PlayerStats.
        currentHealth.Value = playerStats.Health.CurrentHealth;
        maxHealth.Value = playerStats.Health.GetMaxHealth();
    }
}