
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestisce la rappresentazione grafica
/// di un singolo slot dell'inventario.
///
/// Mostra l'icona e la quantità dell'oggetto
/// e comunica quando lo slot viene selezionato.
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [Header("Riferimenti UI")]

    /// <summary>
    /// Pulsante che rappresenta lo slot.
    /// </summary>
    [SerializeField]
    private Button slotButton;

    /// <summary>
    /// Immagine dell'oggetto contenuto.
    /// </summary>
    [SerializeField]
    private Image itemIcon;

    /// <summary>
    /// Testo che mostra la quantità.
    /// </summary>
    [SerializeField]
    private TMP_Text quantityText;

    // Slot attualmente visualizzato.
    private InventorySlot currentSlot;

    // Azione da eseguire quando si seleziona lo slot.
    private Action<InventorySlot> onSelected;

    /// <summary>
    /// Registra l'evento del pulsante.
    /// </summary>
    private void Awake()
    {
        slotButton.onClick.AddListener(HandleClick);
    }

    /// <summary>
    /// Aggiorna la rappresentazione grafica dello slot.
    ///
    /// Se slot è null, mostra una casella vuota.
    /// </summary>
    public void SetSlot(
        InventorySlot slot,
        Action<InventorySlot> selectionCallback)
    {
        currentSlot = slot;
        onSelected = selectionCallback;

        // Controlla se lo slot contiene un oggetto.
        bool occupied =
            slot != null &&
            slot.Item != null &&
            slot.Quantity > 0;

        // Mostra l'icona soltanto se occupato.
        itemIcon.enabled = occupied;

        if (occupied)
        {
            itemIcon.sprite = slot.Item.Icon;
        }

        // Mostra la quantità solo se maggiore di uno.
        quantityText.text =
            occupied && slot.Quantity > 1
                ? slot.Quantity.ToString()
                : string.Empty;

        // Gli slot vuoti non sono selezionabili.
        slotButton.interactable = occupied;
    }

    /// <summary>
    /// Gestisce la selezione dello slot.
    /// Notifica InventoryUI dell'oggetto selezionato.
    /// </summary>
    private void HandleClick()
    {
        if (currentSlot == null)
            return;

        onSelected?.Invoke(currentSlot);
    }
}
