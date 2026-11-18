
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Gestisce l'interfaccia grafica dell'inventario.
///
/// Crea gli slot visivi, aggiorna icone e quantità,
/// mostra i dettagli degli oggetti selezionati
/// e permette di aprire o chiudere l'inventario.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Inventario")]

    /// <summary>
    /// Inventario del giocatore da visualizzare.
    /// </summary>
    [SerializeField]
    private PlayerInventory playerInventory;

    [Header("Interfaccia")]

    /// <summary>
    /// Pannello principale dell'inventario.
    /// </summary>
    [SerializeField]
    private GameObject inventoryPanel;

    /// <summary>
    /// Contenitore della griglia degli slot.
    /// </summary>
    [SerializeField]
    private Transform slotsContainer;

    /// <summary>
    /// Prefab grafico di uno slot.
    /// </summary>
    [SerializeField]
    private InventorySlotUI slotPrefab;

    [Header("Dettagli oggetto")]

    [SerializeField]
    private TMP_Text itemNameText;

    [SerializeField]
    private TMP_Text itemDescriptionText;

    /// <summary>
    /// Lista delle caselle grafiche create.
    /// Non contiene i dati effettivi degli oggetti.
    /// </summary>
    private readonly List<InventorySlotUI> slotViews = new();

    /// <summary>
    /// Inizializza la UI e ascolta
    /// gli aggiornamenti dell'inventario.
    /// </summary>
    private void Start()
    {
        if (playerInventory == null ||
            inventoryPanel == null ||
            slotsContainer == null ||
            slotPrefab == null ||
            itemNameText == null ||
            itemDescriptionText == null)
        {
            Debug.LogError(
                "InventoryUI: riferimenti mancanti nell'Inspector."
            );

            enabled = false;
            return;
        }

        // Si iscrive all'evento dell'inventario.
        playerInventory.OnInventoryChanged += Refresh;

        // Genera e aggiorna la griglia iniziale.
        Refresh();

        // L'inventario parte chiuso.
        inventoryPanel.SetActive(false);
    }

    /// <summary>
    /// Apre o chiude il pannello dell'inventario.
    /// Può essere collegato al pulsante della UI.
    /// </summary>
    public void ToggleInventory()
    {
        if (inventoryPanel == null)
            return;

        bool shouldOpen = !inventoryPanel.activeSelf;

        if (shouldOpen)
            Refresh();

        inventoryPanel.SetActive(shouldOpen);
    }

    /// <summary>
    /// Aggiorna gli slot grafici in base
    /// ai dati contenuti in PlayerInventory.
    /// </summary>
    public void Refresh()
    {
        if (playerInventory == null || slotPrefab == null)
            return;

        // Crea le caselle grafiche mancanti.
        while (slotViews.Count < playerInventory.MaxSlots)
        {
            InventorySlotUI newSlot = Instantiate(
                slotPrefab,
                slotsContainer
            );

            slotViews.Add(newSlot);
        }

        // Elimina le caselle in eccesso
        // se la capacità è stata ridotta.
        while (slotViews.Count > playerInventory.MaxSlots)
        {
            int lastIndex = slotViews.Count - 1;

            InventorySlotUI lastSlot = slotViews[lastIndex];

            slotViews.RemoveAt(lastIndex);

            Destroy(lastSlot.gameObject);
        }

        // Associa ogni casella a uno slot dell'inventario.
        for (int i = 0; i < slotViews.Count; i++)
        {
            InventorySlot inventorySlot =
                i < playerInventory.Slots.Count
                    ? playerInventory.Slots[i]
                    : null;

            slotViews[i].SetSlot(
                inventorySlot,
                SelectItem
            );
        }

        // Evita di mostrare dettagli non più aggiornati.
        ClearSelection();
    }

    /// <summary>
    /// Mostra nome e descrizione dell'oggetto selezionato.
    /// </summary>
    private void SelectItem(InventorySlot slot)
    {
        if (slot == null || slot.Item == null)
            return;

        itemNameText.text = slot.Item.DisplayName;

        itemDescriptionText.text =
            slot.Item.Description;
    }

    /// <summary>
    /// Svuota il pannello dei dettagli.
    /// </summary>
    private void ClearSelection()
    {
        itemNameText.text = "Seleziona un oggetto";
        itemDescriptionText.text = string.Empty;
    }

    /// <summary>
    /// Rimuove la registrazione all'evento
    /// quando il componente viene distrutto.
    /// </summary>
    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= Refresh;
        }
    }
}
