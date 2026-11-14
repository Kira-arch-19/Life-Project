using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(NetworkHealth))]
public class PlayerAttack : NetworkBehaviour
{
    [Header("Attacco")]
    [SerializeField, Min(0.1f)] private float attackRange = 1.5f;
    [SerializeField, Min(0f)] private float attackCooldown = 0.5f;

    // Layer dei bersagli che possiamo colpire.
    [SerializeField] private LayerMask targetLayers;

    private PlayerStats stats;
    private NetworkHealth networkHealth;

    // Il client limita le richieste.
    // Il server controlla separatamente il cooldown effettivo.
    private float nextLocalAttackTime;
    private double nextServerAttackTime;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        networkHealth = GetComponent<NetworkHealth>();
    }

    public override void OnNetworkSpawn()
    {
        nextLocalAttackTime = 0f;
        nextServerAttackTime = 0d;
    }

    // Viene chiamato dal pulsante sul client proprietario.
    public void TryAttack()
    {
        if (!IsSpawned || !IsClient || !IsOwner)
            return;

        if (networkHealth.IsDead ||
            Time.unscaledTime < nextLocalAttackTime)
            return;

        nextLocalAttackTime = Time.unscaledTime + attackCooldown;

        // Inviamo soltanto la richiesta.
        // Il client non decide bersaglio o quantità di danno.
        RequestAttackRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = true)]
    private void RequestAttackRpc()
    {
        // Questi controlli vengono eseguiti sul server.
        if (!IsServer || !IsSpawned)
            return;

        if (stats.Health.IsDead())
            return;

        double now = NetworkManager.ServerTime.Time;

        if (now < nextServerAttackTime)
            return;

        // Anche un attacco a vuoto consuma il cooldown.
        nextServerAttackTime = now + attackCooldown;

        NetworkHealth target = FindClosestTarget();

        if (target == null)
            return;

        PlayerStats targetStats = target.GetComponent<PlayerStats>();

        // Il server usa le proprie statistiche per calcolare il danno.
        int damage = targetStats.DefenseStats.ReduceDamage(
            stats.AttackStats.CurrentDamage
        );

        // NetworkHealth aggiorna Health e i valori sincronizzati.
        target.TakeDamage(damage);

        Debug.Log(
            $"Player {OwnerClientId} colpisce {target.name}: " +
            $"{damage} danni. Vita: {target.CurrentHealth}",
            target
        );

        if (target.IsDead)
            Debug.Log($"{target.name} è morto.", target);
    }

    // Questo metodo viene usato esclusivamente dal server.
    private NetworkHealth FindClosestTarget()
    {
        Vector2 origin = transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            attackRange,
            targetLayers
        );

        NetworkHealth closestTarget = null;
        float closestDistance = float.PositiveInfinity;

        foreach (Collider2D hit in hits)
        {
            // Cerca NetworkHealth anche sul genitore del collider.
            NetworkHealth candidate =
                hit.GetComponentInParent<NetworkHealth>();

            // Esclude noi stessi e oggetti non presenti in rete.
            if (candidate == null ||
                candidate == networkHealth ||
                !candidate.IsSpawned)
                continue;

            PlayerStats candidateStats =
                candidate.GetComponent<PlayerStats>();

            if (candidateStats == null ||
                candidateStats.Health == null ||
                candidateStats.DefenseStats == null)
                continue;

            // Controlla direttamente la Health del server.
            if (candidateStats.Health.IsDead())
                continue;

            Vector2 closestPoint = hit.ClosestPoint(origin);
            float distance = (closestPoint - origin).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = candidate;
            }
        }

        // Restituisce un solo bersaglio anche con più collider.
        return closestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        // Il cerchio rappresenta l'area dell'attacco nel mondo 2D.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}