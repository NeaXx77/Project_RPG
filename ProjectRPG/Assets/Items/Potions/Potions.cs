using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Potion", menuName = "Potion")]
public class Potions : ScriptableObject
{
    public int id;
    public float baseRecovery;
    public Sprite itemSprite;
    public string potionName;
    public int stack;
}
