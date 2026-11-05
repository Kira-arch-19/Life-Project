using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerAestheticsManager : MonoBehaviour
{
    

    public List<AestheticCategory> aestheticCategories = new List<AestheticCategory>();
    public Dictionary<AestheticType, int> selectedIDs = new Dictionary<AestheticType, int>();
    

    public void SetAesthetic(AestheticType type, int id)
    {
        AestheticCategory category = aestheticCategories.FirstOrDefault(c => c.aestheticType == type);
        if(category == null || id < 0 || id >= category.spriteList.Count)
        {
            Debug.LogError($"Invalid aesthetic type or ID: {type}, {id}");
            return;
        }
        selectedIDs[type] = id;

    }
    private int getAestheticID(AestheticType type)
    {
        return selectedIDs.TryGetValue(type, out int id) ? id : 0;
        
    }
    
}
