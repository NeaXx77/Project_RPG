using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Orb", menuName = "Orbs")]
public class Orbs : ScriptableObject
{
    public int id;
    public int armorStat;
    public int levelMax;
    public int expNeeded;
    public Sprite itemSprite;
    public enum Type{
        resist,
        speed,
        weapons
    }
    public Type type;
    [System.Flags]
    public enum ElementResistance{
        none = 1,
        fire = 2,
        ice = 4,
        toxic = 8
    }
    public ElementResistance resistance;

}