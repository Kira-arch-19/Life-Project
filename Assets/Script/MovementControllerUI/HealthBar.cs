using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBarUI : MonoBehaviour
{
    // Il personaggio di cui mostrare la salute.
    [SerializeField] private PlayerStats target;

    private Slider healthSlider;

    private void Awake()
    {
        healthSlider = GetComponent<Slider>();

        // La barra serve a visualizzare la vita.
        healthSlider.interactable = false;
        healthSlider.minValue = 0f;
        healthSlider.wholeNumbers = true;
    }

    private void Update()
    {
        if (target == null || target.Health == null)
            return;

        // Aggiorna anche il massimo, se cambia durante la partita.
        healthSlider.maxValue = target.Health.GetMaxHealth();
        healthSlider.value = target.Health.CurrentHealth;
    }
}