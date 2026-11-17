
using System;
using UnityEngine;

/// <summary>
/// Rappresenta uno slot occupato dell'inventario.
/// Contiene un tipo di oggetto e la sua quantità.
/// </summary>
[Serializable]
public class InventorySlot
{
    [SerializeField] private ItemDefinition item;
    [SerializeField] private int quantity;

    /// <summary>
    /// Oggetto contenuto nello slot.
    /// </summary>
    public ItemDefinition Item => item;

    /// <summary>
    /// Quantità presente nello slot.
    /// </summary>
    public int Quantity => quantity;

    /// <summary>
    /// Spazio ancora disponibile nello stack.
    /// </summary>
    public int FreeSpace => item.MaxStack - quantity;
   /// <summary>
   /// Restituisce true se lo slot è vuoto.
   /// </summary>
    public bool IsEmpty => quantity <= 0;
    /// <summary>
    /// Crea uno slot con un oggetto e una quantità.
    /// </summary>
    public InventorySlot(ItemDefinition item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
    /// <summary>
/// Riduce la quantità presente nello slot.
/// Restituisce false se la quantità richiesta
/// non è valida oppure supera quella disponibile.
/// </summary>
public bool TryDecrease(int amount)
{
    if (amount <= 0 || amount >quantity )
        return false;

    quantity -= amount;

    return true;
}
    /// <summary>
    /// Aumenta la quantità nello slot.
    /// Il chiamante deve rispettare MaxStack.
    /// </summary>
    public void Increase(int amount)
    {
        quantity += amount;
    }

}
