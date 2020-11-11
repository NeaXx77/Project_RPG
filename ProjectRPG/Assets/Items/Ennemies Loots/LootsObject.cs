using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootObject", menuName = "Loot Object")]
public class LootsObject : ScriptableObject
{
    public int id;
    public Sprite itemSprite;
    public int stack;
    public string lootName;
}
