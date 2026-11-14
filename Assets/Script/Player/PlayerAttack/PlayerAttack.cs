using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerAttack2D : MonoBehaviour
{
    [Header("Attacco")]
    [SerializeField, Min(0.1f)] private float attackRange = 1.5f;
    [SerializeField, Min(0f)] private float attackCooldown = 0.5f;

    // Seleziona nell'Inspector il layer dei bersagli.
    [SerializeField] private LayerMask targetLayers;

    private PlayerStats stats;
    private float nextAttackTime;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    // Metodo pubblico collegato al pulsante UI.
    public void TryAttack()
    {
        // Un personaggio morto non può attaccare.
        // Il cooldown impedisce attacchi troppo ravvicinati.
        if (stats.Health.IsDead() || Time.time < nextAttackTime)
            return;

        // Anche un attacco a vuoto consuma il cooldown.
        nextAttackTime = Time.time + attackCooldown;

        PlayerStats target = FindClosestTarget();

        if (target == null)
            return;

        // Calcola il danno dopo la difesa del bersaglio.
        int damage = target.DefenseStats.ReduceDamage(
            stats.AttackStats.CurrentDamage);

        // Applica il danno alla salute.
        target.Health.TakeDamage(damage);

        Debug.Log(
            $"Colpito {target.name}: {damage} danni. " +
            $"Vita rimasta: {target.Health.CurrentHealth}",
            target);

        if (target.Health.IsDead())
            Debug.Log($"{target.name} è morto.", target);
    }

    private PlayerStats FindClosestTarget()
    {
        Vector2 origin = transform.position;

        // Cerca i collider nel raggio e nei layer selezionati.
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            attackRange,
            targetLayers);

        PlayerStats closestTarget = null;
        float closestDistance = float.PositiveInfinity;

        foreach (Collider2D hit in hits)
        {
            // Funziona anche se il collider è su un oggetto figlio.
            PlayerStats candidate = hit.GetComponentInParent<PlayerStats>();

            // Ignora oggetti senza statistiche, noi stessi e bersagli morti.
            if (candidate == null || candidate == stats)
                continue;

            if (candidate.Health == null || candidate.DefenseStats == null)
                continue;

            if (candidate.Health.IsDead())
                continue;

            // Misura la distanza dal punto più vicino del collider.
            Vector2 closestPoint = hit.ClosestPoint(origin);
            float distance = (closestPoint - origin).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = candidate;
            }
        }

        // Restituisce un solo bersaglio, anche se ha diversi collider.
        return closestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        // Mostra il raggio nella finestra Scene quando selezioni il Player.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}