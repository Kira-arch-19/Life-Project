
using UnityEngine;

/// <summary>
/// Definisce un tipo di oggetto presente in LIFE PROJECT.
///
/// È uno ScriptableObject: contiene i dati condivisi
/// di un oggetto, non la quantità posseduta dal giocatore.
/// </summary>
[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "LIFE PROJECT/Inventory/Item"
)]
public class ItemDefinition : ScriptableObject
{
    /// <summary>
    /// Identificativo stabile e univoco dell'oggetto.
    /// Verrà usato anche dal backend .NET.
    /// Esempio: wood, stone, iron_sword.
    /// </summary>
    [SerializeField] private string itemId;

    /// <summary>
    /// Nome mostrato nell'interfaccia.
    /// </summary>
    [SerializeField] private string displayName;

    /// <summary>
    /// Icona mostrata nell'inventario.
    /// </summary>
    [SerializeField] private Sprite icon;

    /// <summary>
    /// Numero massimo di oggetti nello stesso slot.
    /// </summary>
    [SerializeField, Min(1)] private int maxStack = 99;
    /// <summary>
    /// Descrizione degli item
    /// </summary>
    [SerializeField,TextArea(2, 5)] private string descriptionItem;

    // Proprietà pubbliche di sola lettura.
    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => descriptionItem;
    public Sprite Icon => icon;
    public int MaxStack => maxStack;
}
