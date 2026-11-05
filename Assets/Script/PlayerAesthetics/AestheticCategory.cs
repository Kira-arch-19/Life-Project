using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class AestheticCategory
{
    public AestheticType aestheticType;
    public List<Sprite> spriteList=new ();
}
public enum AestheticType
{
    Skin,
    Hair,
    Eyes,
    Pants,
    Armor,
    Shoes,
    Weapon
}