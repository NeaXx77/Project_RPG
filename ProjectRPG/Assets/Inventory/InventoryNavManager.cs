using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryNavManager : MonoBehaviour{
    public GameObject meleeWeaponScroll;
    public GameObject rangedWeaponScroll;
    public Button[] eachFirstInventoryButton;//0 = potion, 1 = loot, 2 = first melee, 3 = first ranged, 
                                                                    //4 = meleeweaponslot, 5 = rangedweaponslot
    public InventoryAnimationManager inventoryAnimManager;
    PlayerInputController input;
    private void Awake() {
        input = FindObjectOfType<PlayerInputController>();
        input.inventoryNavManager = this;
    }

    //call on button click
    public void SetActiveMeleeWeaponScroll(bool state){
        meleeWeaponScroll.SetActive(state);
        if(state){
            eachFirstInventoryButton[2].Select();
        }else
        {
            eachFirstInventoryButton[4].Select();
        }
    }
    //Call on button click
    public void SetActiveRangedWeaponScroll(bool state){
        rangedWeaponScroll.SetActive(state);
        if(state){
            eachFirstInventoryButton[3].Select();
        }else
        {
            eachFirstInventoryButton[5].Select();
        }
    }
    //Call on switch button pressed
    public void OnSwitchPanel(){
        if (inventoryAnimManager.GetIsOverviewPanel())
        {
            eachFirstInventoryButton[4].Select();
            print("overview");
        }else{
            if(input.GetInventoryFocus() == PlayerInputController.inventoryFocus.potion){
                eachFirstInventoryButton[0].Select();
            }else{
                eachFirstInventoryButton[1].Select();
            }
        }
    }
}

