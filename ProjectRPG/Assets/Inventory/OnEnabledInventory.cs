using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnabledInventory : MonoBehaviour
{
    public bool potion,loot,weapon;
    private void Update() {
        if(loot)
            GetComponentInParent<InventoryUI>().updateLootUI();
        else if(potion)
            GetComponentInParent<InventoryUI>().updatePotionUI();    
    }
}
