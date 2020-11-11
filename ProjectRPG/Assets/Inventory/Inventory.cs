using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxStack;
    public static Inventory instance;
    [HideInInspector]
    public List<LootsObject> lootList;
    public int nbLootItem=0;
    [HideInInspector]
    public List<Potions> potionList;
    public int nbPotionItem=0;
    [HideInInspector]
    public List<Weapon> weaponList;
    public int nbWeaponItem=0;
    private void Awake() {
        //get all slot in each inventory spaces
        instance = this;
        lootList = new List<LootsObject>();
        potionList = new List<Potions>();
        weaponList = new List<Weapon>();
    }
    //if the object has been added succesfully, return true else return false
    public bool addInventory(LootsObject loot){
        for (int i = 0; i < nbLootItem; i++)
        {
            //test if there is already this item
            if (lootList[i].id == loot.id)
            {
                //faire une fonction qui incremente juste
                if(lootList[i].stack < maxStack){
                    lootList[i].stack++;
                    return true;
                }else{
                    return false;    
                }
            }
        }
        lootList.Add(loot);
        nbLootItem++;
        return true;
    }
    public bool addInventory(Potions potion){
        for (int i = 0; i < nbPotionItem; i++)
        {
            //test if there is already this item
            if (potionList[i].id == potion.id)
            {
                //faire une fonction qui incremente juste
                if(potionList[i].stack < maxStack){
                    potionList[i].stack++;
                    return true;
                }else{
                    return false;    
                }
            }
        }
        potionList.Add(potion);
        nbPotionItem++;
        return true;
    }
    public bool addInventory(Weapon weapon){
        weaponList.Add(weapon);
        return true;
    }
    public bool lootIsFull(LootsObject loot){
        for (int i = 0; i < nbLootItem; i++)
        {
            if (lootList[i].id == loot.id && lootList[i].stack >= maxStack){
                return true;
            }
        }
        return false;
    }
}
