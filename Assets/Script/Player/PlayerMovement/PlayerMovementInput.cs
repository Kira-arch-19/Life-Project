using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementInput : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;

    private NetworkPlayer networkPlayer;

    private void Awake()
    {
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    private void OnEnable()
    {
        if (networkPlayer == null ||
            moveAction == null ||
            moveAction.action == null)
        {
            Debug.LogError(
                "Assegna l'azione Move e aggiungi NetworkPlayer.",
                this);

            enabled = false;
            return;
        }

        moveAction.action.Enable();
    }

    private void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // NetworkPlayer invierà questa direzione al server.
        networkPlayer.SetMoveInput(input);
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();

        if (networkPlayer != null)
            networkPlayer.SetMoveInput(Vector2.zero);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && networkPlayer != null)
            networkPlayer.SetMoveInput(Vector2.zero);
    }
}