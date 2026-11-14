using Unity.Netcode;
using UnityEngine;

public class PlayerAttackBTN : MonoBehaviour
{
    // Collega questo metodo all'On Click del pulsante.
    public void Attack()
    {
        NetworkManager manager = NetworkManager.Singleton;

        // Non possiamo attaccare prima della connessione.
        if (manager == null || !manager.IsClient ||
            !manager.IsConnectedClient)
            return;

        // Recupera il personaggio posseduto da questo client.
        NetworkObject player = manager.LocalClient.PlayerObject;

        if (player == null)
            return;

        if (player.TryGetComponent(out PlayerAttack attack))
        {
            attack.TryAttack();
        }
    }
}