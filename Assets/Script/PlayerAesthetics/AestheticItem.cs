using UnityEngine;

[CreateAssetMenu(
    fileName = "NewAestheticItem",
    menuName = "Life Project/Aesthetics/Item"
)]
public class AestheticItem : ScriptableObject
{
    [SerializeField] private string itemID;
    [SerializeField] private AestheticType type;

    [Header("Animation Sprites")]
    [SerializeField] private Sprite[] idleDown;
    [SerializeField] private Sprite[] idleUp;
    [SerializeField] private Sprite[] idleLeft;
    [SerializeField] private Sprite[] idleRight;

    [SerializeField] private Sprite[] walkDown;
    [SerializeField] private Sprite[] walkUp;
    [SerializeField] private Sprite[] walkLeft;
    [SerializeField] private Sprite[] walkRight;

    public string ItemID => itemID;
    public AestheticType Type => type;
}