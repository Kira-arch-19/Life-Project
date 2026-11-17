
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Test automatici EditMode per verificare
/// il comportamento dell'inventario locale.
/// </summary>
public class InventoryTests
{
    private GameObject player;
    private PlayerInventory inventory;
    private ItemDefinition wood;

    /// <summary>
    /// Prepara un inventario vuoto e un oggetto
    /// di prova prima di ogni test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        player = new GameObject("TestPlayer");

        inventory = player.AddComponent<PlayerInventory>();

        // Crea un ItemDefinition temporaneo.
        // Il MaxStack predefinito è 99.
        wood = ScriptableObject.CreateInstance<ItemDefinition>();
    }

    /// <summary>
    /// Elimina gli oggetti temporanei dopo ogni test.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(wood);
    }

    /// <summary>
    /// Due aggiunte dello stesso oggetto devono
    /// utilizzare un solo slot, se c'è spazio.
    /// </summary>
    [Test]
    public void TryAdd_SameItem_MergesStacks()
    {
        Assert.That(inventory.TryAdd(wood, 5), Is.True);
        Assert.That(inventory.TryAdd(wood, 5), Is.True);

        Assert.That(inventory.GetQuantity(wood), Is.EqualTo(10));
        Assert.That(inventory.UsedSlots, Is.EqualTo(1));
    }

    /// <summary>
    /// Quando il primo stack raggiunge MaxStack,
    /// gli oggetti rimanenti occupano un nuovo slot.
    /// </summary>
    [Test]
    public void TryAdd_ExceedsMaxStack_CreatesNewSlot()
    {
        Assert.That(inventory.TryAdd(wood, 100), Is.True);

        Assert.That(inventory.UsedSlots, Is.EqualTo(2));
        Assert.That(inventory.Slots[0].Quantity, Is.EqualTo(99));
        Assert.That(inventory.Slots[1].Quantity, Is.EqualTo(1));
    }

    /// <summary>
    /// Quando la quantità arriva a zero,
    /// lo slot deve scomparire dall'inventario.
    /// </summary>
    [Test]
    public void TryRemove_AllItems_RemovesEmptySlot()
    {
        inventory.TryAdd(wood, 5);

        bool removed = inventory.TryRemove(wood, 5);

        Assert.That(removed, Is.True);
        Assert.That(inventory.GetQuantity(wood), Is.Zero);
        Assert.That(inventory.UsedSlots, Is.Zero);
    }

    /// <summary>
    /// Se il giocatore non possiede abbastanza oggetti,
    /// nessuna quantità deve essere rimossa.
    /// </summary>
    [Test]
    public void TryRemove_InsufficientItems_DoesNotModifyInventory()
    {
        inventory.TryAdd(wood, 5);

        bool removed = inventory.TryRemove(wood, 10);

        Assert.That(removed, Is.False);
        Assert.That(inventory.GetQuantity(wood), Is.EqualTo(5));
    }

    /// <summary>
    /// Verifica che non sia possibile aggiungere
    /// oggetti quando l'inventario è pieno.
    /// </summary>
    [Test]
    public void TryAdd_FullInventory_ReturnsFalse()
    {
        // Riduce la capacità a un solo slot.
        inventory.TryDecreaseSpaceInventorySlot(19);

        inventory.TryAdd(wood, 99);

        bool added = inventory.TryAdd(wood, 1);

        Assert.That(added, Is.False);
        Assert.That(inventory.GetQuantity(wood), Is.EqualTo(99));
    }

    /// <summary>
    /// Verifica che uno slot non possa contenere
    /// più oggetti del limite MaxStack.
    /// </summary>
    [Test]
    public void InventorySlot_CannotExceedMaxStack()
    {
        InventorySlot slot = new InventorySlot(wood, 99);

        bool increased = slot.TryIncrease(1);

        Assert.That(increased, Is.False);
        Assert.That(slot.Quantity, Is.EqualTo(99));
    }
}
