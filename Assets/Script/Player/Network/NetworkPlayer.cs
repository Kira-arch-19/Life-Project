using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerMovementInput))]
public class NetworkPlayer : NetworkBehaviour
{
    // Per il prototipo inviamo l'input 20 volte al secondo.
// Invia l'input circa 60 volte al secondo.
    private const float InputInterval = 1f / 60f;
    // Se gli input si interrompono, il server ferma il personaggio.
    private const float InputTimeout = 0.3f;

    private PlayerMovement movement;
    private PlayerMovementInput inputReader;

    private Vector2 localInput;
    private float nextInputTime;
    private float lastReceivedInputTime;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        inputReader = GetComponent<PlayerMovementInput>();
    }

    public override void OnNetworkSpawn()
    {
        // La fisica del movimento viene eseguita dal server.
        movement.enabled = IsServer;

        // Ogni client legge i controlli solo per il proprio personaggio.
        inputReader.enabled = IsClient && IsOwner;

        localInput = Vector2.zero;
        nextInputTime = 0f;

        if (IsServer)
        {
            lastReceivedInputTime = Time.unscaledTime;

            // Posizione iniziale temporanea.
            int slot = (int)(OwnerClientId % 8);

            float x = (slot % 4) * 2f - 3f;
            float y = (slot / 4) * 2f;

            transform.position = new Vector3(x, y, 0f);
        }
    }

    // Chiamato dal lettore di input sul telefono o PC del giocatore.
    public void SetMoveInput(Vector2 input)
    {
        if (!IsSpawned || !IsOwner)
            return;

        localInput = Vector2.ClampMagnitude(input, 1f);
    }

    private void Update()
    {
        if (!IsSpawned)
            return;

        // Il proprietario invia periodicamente la direzione,
        // incluso zero quando rilascia il joystick.
        if (IsClient && IsOwner && Time.unscaledTime >= nextInputTime)
        {
            nextInputTime = Time.unscaledTime + InputInterval;
            SubmitMovementRpc(localInput);
        }

        // Evita che il personaggio continui a muoversi
        // quando il client smette di inviare aggiornamenti.
        if (IsServer &&
            Time.unscaledTime - lastReceivedInputTime > InputTimeout)
        {
            movement.SetMoveInput(Vector2.zero);
        }
    }

    // Netcode permette l'invio solo al proprietario di questo oggetto.
    [Rpc(SendTo.Server, RequireOwnership = true)]
    private void SubmitMovementRpc(Vector2 input)
    {
        // Rifiuta valori numerici non validi.
        if (float.IsNaN(input.x) || float.IsNaN(input.y) ||
            float.IsInfinity(input.x) || float.IsInfinity(input.y))
        {
            return;
        }

        lastReceivedInputTime = Time.unscaledTime;

        // Il server limita l'intensità della direzione.
        // La velocità effettiva viene letta dalle statistiche sul server.
        movement.SetMoveInput(Vector2.ClampMagnitude(input, 1f));
    }

    public override void OnNetworkDespawn()
    {
        inputReader.enabled = false;
        movement.enabled = false;
        localInput = Vector2.zero;
    }
}