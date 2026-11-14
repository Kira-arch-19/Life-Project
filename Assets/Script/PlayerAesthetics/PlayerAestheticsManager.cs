using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAestheticsManager : MonoBehaviour
{
    public List<AestheticCategory> aestheticCategories = new();
    public Dictionary<AestheticType, int> selectedIDs = new();

    private void Awake()
    {
        foreach (AestheticCategory category in aestheticCategories)
        {
            if (category == null || category.spriteRenderer == null ||
                category.spriteList == null || category.spriteList.Count == 0)
                continue;

            int id = selectedIDs.TryGetValue(category.aestheticType, out int selectedId)
                ? selectedId
                : 0;

            SetAesthetic(category.aestheticType, id);
        }
    }

    public void SetAesthetic(AestheticType type, int id)
    {
        AestheticCategory category = aestheticCategories.FirstOrDefault(
            entry => entry != null && entry.aestheticType == type);

        if (category?.spriteList == null || id < 0 || id >= category.spriteList.Count)
        {
            Debug.LogError($"Categoria o ID non valido: {type}, {id}", this);
            return;
        }

        selectedIDs[type] = id;

        if (category.spriteRenderer != null)
            category.spriteRenderer.sprite = category.spriteList[id];
    }
}