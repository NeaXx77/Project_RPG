using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    //WeaponSlots et weaponInventory serviront pour le bandeau déroulant du slot weapon
    public static InventoryUI instance;
    public GameObject potionInventory,weaponInventory,lootInventory;
    private inventorySlots[] lootObjectSlots;
    private inventorySlots[] potionSlots;
    private inventorySlots[] weaponSlots;
    Inventory inventory;
    public void Awake() {
        instance = this;
        inventory = Inventory.instance;
        lootObjectSlots = lootInventory.GetComponentsInChildren<inventorySlots>();
        potionSlots = potionInventory.GetComponentsInChildren<inventorySlots>();
    }
    public void updateLootUI(){
        for (int i = 0; i < lootObjectSlots.Length; i++)
        {
            /*si on a pas depassé le dernier element de la list */
            if(i < inventory.nbLootItem  && inventory.lootList[i] != null){
                lootObjectSlots[i].addItem(inventory.lootList[i]);
            }else{
                lootObjectSlots[i].clear();
            }
        }
    }
    public void updatePotionUI(){
        for (int i = 0; i < potionSlots.Length; i++)
        {
            /*si on a pas depassé le dernier element de la list */
            if(i < inventory.nbPotionItem  && inventory.potionList[i] != null){
                potionSlots[i].addItem(inventory.potionList[i]);
            }else{
                potionSlots[i].clear();
            }
        }
    }
    public void updateWeaponUI(){
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            /*si on a pas depassé le dernier element de la list */
            if(i < inventory.nbWeaponItem  && inventory.weaponList[i] != null){
                weaponSlots[i].addItem(inventory.weaponList[i]);
            }else{
                weaponSlots[i].clear();
            }
        }
    }
}
