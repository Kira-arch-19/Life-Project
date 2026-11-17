
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestisce l'inventario locale del giocatore.
///
/// Si occupa di:
/// - Aggiungere oggetti.
/// - Rimuovere oggetti.
/// - Gestire gli stack.
/// - Controllare la capacità.
/// - Notificare i cambiamenti alla UI.
///
/// Per ora funziona offline.
/// Successivamente le modifiche saranno autorizzate
/// dal server Unity tramite Netcode.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("Configurazione inventario")]

    /// <summary>
    /// Numero massimo di slot disponibili.
    /// </summary>
    [SerializeField, Min(1)]
    private int maxSlots = 20;

    /// <summary>
    /// Lista degli slot occupati.
    /// Gli slot vuoti non vengono conservati.
    /// </summary>
    [SerializeField]
    private List<InventorySlot> slots = new();

    [Header("Test")]

    /// <summary>
    /// Oggetto utilizzato nei test dall'Inspector.
    /// </summary>
    [SerializeField]
    private ItemDefinition testItem;

    // ==========================================
    // PROPRIETÀ
    // ==========================================

    /// <summary>
    /// Capacità massima dell'inventario.
    /// </summary>
    public int MaxSlots => maxSlots;

    /// <summary>
    /// Numero di slot attualmente occupati.
    /// </summary>
    public int UsedSlots => slots.Count;

    /// <summary>
    /// Numero di slot ancora disponibili.
    /// </summary>
    public int FreeSlots => maxSlots - slots.Count;

    /// <summary>
    /// Consente di leggere gli slot dall'esterno,
    /// senza esporre direttamente la lista modificabile.
    /// </summary>
    public IReadOnlyList<InventorySlot> Slots => slots;

    /// <summary>
    /// Evento emesso quando l'inventario cambia.
    /// La UI potrà iscriversi a questo evento.
    /// </summary>
    public event Action OnInventoryChanged;

    // ==========================================
    // AGGIUNTA OGGETTI
    // ==========================================

    /// <summary>
    /// Prova ad aggiungere una quantità di oggetti.
    ///
    /// Riempie prima gli stack esistenti e,
    /// se necessario, crea nuovi slot.
    ///
    /// Restituisce false se non c'è spazio sufficiente.
    /// Non effettua aggiunte parziali.
    /// </summary>
    public bool TryAdd(ItemDefinition item, int amount)
    {
        // Verifica che i parametri siano validi.
        if (item == null || amount <= 0)
            return false;

        // Calcola la capacità ancora disponibile.
        // long evita overflow nel calcolo dello spazio.
        long freeSpace =
            (long)(maxSlots - slots.Count) * item.MaxStack;

        // Aggiunge lo spazio libero degli stack esistenti.
        foreach (InventorySlot slot in slots)
        {
            if (slot.Item == item)
                freeSpace += slot.FreeSpace;
        }

        // Rifiuta l'aggiunta se non c'è abbastanza spazio.
        if (freeSpace < amount)
            return false;

        int remaining = amount;

        // Prima riempie gli stack già presenti.
        foreach (InventorySlot slot in slots)
        {
            if (slot.Item != item || slot.FreeSpace <= 0)
                continue;

            int toAdd = Mathf.Min(
                remaining,
                slot.FreeSpace
            );

            slot.TryIncrease(toAdd);
            remaining -= toAdd;

            if (remaining == 0)
                break;
        }

        // Poi crea nuovi slot, se necessari.
        while (remaining > 0)
        {
            int toAdd = Mathf.Min(
                remaining,
                item.MaxStack
            );

            slots.Add(new InventorySlot(item, toAdd));

            remaining -= toAdd;
        }

        // Notifica il cambiamento.
        OnInventoryChanged?.Invoke();

        return true;
    }

    // ==========================================
    // RIMOZIONE OGGETTI
    // ==========================================

    /// <summary>
    /// Prova a rimuovere una quantità di oggetti.
    ///
    /// Può rimuovere oggetti da più stack.
    /// Elimina automaticamente gli slot vuoti.
    ///
    /// Restituisce false se la quantità disponibile
    /// non è sufficiente.
    /// Non effettua rimozioni parziali.
    /// </summary>
    public bool TryRemove(ItemDefinition item, int amount)
    {
        // Verifica i parametri.
        if (item == null || amount <= 0)
            return false;

        // Non permette di rimuovere più oggetti
        // di quelli realmente posseduti.
        if (GetQuantity(item) < amount)
            return false;

        int remaining = amount;

        // Scorre gli slot dall'ultimo al primo.
        // In questo modo possiamo eliminarli
        // senza compromettere il ciclo.
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            InventorySlot slot = slots[i];

            // Ignora gli oggetti diversi.
            if (slot.Item != item)
                continue;

            // Calcola la quantità da rimuovere
            // dallo slot corrente.
            int toRemove = Mathf.Min(
                remaining,
                slot.Quantity
            );

            // Diminuisce realmente la quantità.
            slot.TryDecrease(toRemove);

            remaining -= toRemove;

            // Se lo stack è vuoto, rimuove lo slot.
            if (slot.IsEmpty)
                slots.RemoveAt(i);

            // Termina quando la richiesta è soddisfatta.
            if (remaining == 0)
                break;
        }

        // Notifica il cambiamento.
        OnInventoryChanged?.Invoke();

        return true;
    }

    // ==========================================
    // CONSULTAZIONE INVENTARIO
    // ==========================================

    /// <summary>
    /// Restituisce la quantità totale di un oggetto.
    /// Somma le quantità presenti nei vari stack.
    /// </summary>
    public int GetQuantity(ItemDefinition item)
    {
        if (item == null)
            return 0;

        int total = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.Item == item)
                total += slot.Quantity;
        }

        return total;
    }

    // ==========================================
    // GESTIONE CAPACITÀ
    // ==========================================

    /// <summary>
    /// Aumenta il numero massimo di slot.
    /// Restituisce false se l'importo non è valido.
    /// </summary>
    public bool TryIncreaseSpaceInventorySlot(int amount)
    {
        if (amount <= 0 || maxSlots > int.MaxValue - amount)
            return false;

        maxSlots += amount;

        OnInventoryChanged?.Invoke();

        return true;
    }

    /// <summary>
    /// Diminuisce il numero massimo di slot.
    ///
    /// Non permette di eliminare slot occupati
    /// o di scendere sotto uno slot disponibile.
    /// </summary>
    public bool TryDecreaseSpaceInventorySlot(int amount)
    {
        if (amount <= 0)
            return false;

        int newMaxSlots = maxSlots - amount;

        if (newMaxSlots < 1 || newMaxSlots < slots.Count)
            return false;

        maxSlots = newMaxSlots;

        OnInventoryChanged?.Invoke();

        return true;
    }

    // ==========================================
    // TEST LOCALI
    // ==========================================

    /// <summary>
    /// Test temporaneo per aggiungere cinque oggetti.
    /// </summary>
    [ContextMenu("TEST / Aggiungi 5 oggetti")]
    private void TestAdd()
    {
        bool added = TryAdd(testItem, 5);

        if (!added)
        {
            Debug.LogWarning(
                "Impossibile aggiungere oggetti."
            );
            return;
        }

        Debug.Log(
            $"Oggetti posseduti: {GetQuantity(testItem)}"
        );
    }

    /// <summary>
    /// Test temporaneo per rimuovere tre oggetti.
    /// </summary>
    [ContextMenu("TEST / Rimuovi 3 oggetti")]
    private void TestRemove()
    {
        bool removed = TryRemove(testItem, 3);

        if (!removed)
        {
            Debug.LogWarning(
                "Oggetti insufficienti."
            );
            return;
        }

        Debug.Log(
            $"Oggetti rimasti: {GetQuantity(testItem)}"
        );
    }
}
