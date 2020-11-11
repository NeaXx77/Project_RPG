using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBase", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public int id;
    public int sellPrice,buyPrice;
    public int baseDamage,levelRatioDamage;
    public string itemName;
    public Sprite icon;
    public enum Rarity
    {common, uncommon, rare, legendary, unique};
    public Rarity rarity;
    public enum Type
    {sword, bow, chakram, claws};
    public Type type;
    public AnimationClip[] animations;
}
