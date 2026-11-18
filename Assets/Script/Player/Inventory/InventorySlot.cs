
using System;
using UnityEngine;

/// <summary>
/// Rappresenta uno stack di oggetti nell'inventario.
/// Gestisce il tipo di oggetto e la quantità contenuta.
///
/// Non gestisce la lista degli slot:
/// questa responsabilità appartiene a PlayerInventory.
/// </summary>
[Serializable]
public class InventorySlot
{
    [SerializeField] private ItemDefinition item;
    [SerializeField] private int quantity;

    /// <summary>
    /// Oggetto presente nello slot.
    /// </summary>
    public ItemDefinition Item => item;

    /// <summary>
    /// Quantità attuale di oggetti.
    /// </summary>
    public int Quantity => quantity;

    /// <summary>
    /// Indica se lo slot è vuoto.
    /// </summary>
    public bool IsEmpty => quantity <= 0;

    /// <summary>
    /// Spazio disponibile nello stack.
    /// </summary>
    public int FreeSpace =>
        item == null
            ? 0
            : Mathf.Max(0, item.MaxStack - quantity);

    /// <summary>
    /// Crea uno slot verificando che l'oggetto esista
    /// e che la quantità rispetti MaxStack.
    /// </summary>
    public InventorySlot(ItemDefinition item, int quantity)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (quantity <= 0 || quantity > item.MaxStack)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        this.item = item;
        this.quantity = quantity;
    }

    /// <summary>
    /// Prova ad aumentare la quantità.
    /// Non permette di superare MaxStack.
    /// </summary>
    public bool TryIncrease(int amount)
    {
        if (amount <= 0 || amount > FreeSpace)
            return false;

        quantity += amount;
        return true;
    }

    /// <summary>
    /// Prova a diminuire la quantità.
    /// Non permette di rimuovere più oggetti
    /// di quelli presenti nello slot.
    /// </summary>
    public bool TryDecrease(int amount)
    {
        if (amount <= 0 || amount > quantity)
            return false;

        quantity -= amount;
        return true;
    }
}
