using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerMovementInput : MonoBehaviour
{
    // Assegna nell'Inspector l'azione Move.
    [SerializeField] private InputActionReference moveAction;

    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (moveAction == null || moveAction.action == null)
        {
            Debug.LogError("Assegna l'azione Move nell'Inspector.", this);
            enabled = false;
            return;
        }

        // Questo componente gestisce l'attivazione dell'azione Move.
        moveAction.action.Enable();
    }

    private void Update()
    {
        // La stessa azione riceve input da tastiera e joystick virtuale.
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        movement.SetMoveInput(input);
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();

        if (movement != null)
            movement.SetMoveInput(Vector2.zero);
    }
}