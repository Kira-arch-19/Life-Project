using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private PlayerStats stats;
    private Vector2 moveInput;    
    // Direzione verso cui guarda il personaggio.
    // Rimane memorizzata anche quando si ferma.
    public Vector2 FacingDirection { get; private set; } = Vector2.down;

   // Indica se il personaggio sta tentando di muoversi.
    public bool IsMoving =>
        moveInput.sqrMagnitude > 0f &&
        stats.MvSpeed.CurrentSpeed > 0f &&
        !stats.Health.IsDead();
        
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    // Riceve la direzione da tastiera, joystick o futura logica di rete.
    public void SetMoveInput(Vector2 input)
    {
        // Limita la lunghezza a 1:
        // la diagonale non rende il personaggio più veloce.
        // Un joystick inclinato parzialmente mantiene un valore inferiore a 1.
        moveInput = Vector2.ClampMagnitude(input, 1f);

        if (moveInput.sqrMagnitude == 0f)
            return;

        // Memorizza una delle quattro direzioni cardinali.
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            FacingDirection = moveInput.x > 0f
                ? Vector2.right
                : Vector2.left;
        }
        else
        {
            FacingDirection = moveInput.y > 0f
                ? Vector2.up
                : Vector2.down;
        }
    }

    private void FixedUpdate()
    {
        // Il personaggio morto rimane fermo.
        if (stats.Health.IsDead())
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        // Rigidbody2D usa unità al secondo.
        // Qui non serve moltiplicare per deltaTime.
        body.linearVelocity = moveInput * stats.MvSpeed.CurrentSpeed;
    }

    private void OnDisable()
    {
        // Evita che il personaggio continui a muoversi
        // dopo la disattivazione del componente.
        moveInput = Vector2.zero;

        if (body != null)
            body.linearVelocity = Vector2.zero;
    }

}
