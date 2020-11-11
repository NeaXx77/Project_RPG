using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAnimationManager : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    GameManage manager;
    bool isOverviewPanel = true;
    private void Awake() {
        manager = FindObjectOfType<GameManage>();
        PlayerInputController.instance.inventoryAnim = this;
        anim = GetComponent<Animator>();
    }
    public void OpenInventory(){
        anim.SetTrigger("Open");
    }
    public void CloseInventory(){
        anim.SetTrigger("Close");
    }
    public void SwitchInventory(){
        if(isOverviewPanel){
            anim.SetTrigger("IntoBag");
        }else{
            anim.SetTrigger("IntoOverview");
        }
        isOverviewPanel = !isOverviewPanel;
    }
    public void DisabledInventoryUI(){
        manager.inventoryObject.SetActive(false);
        isOverviewPanel = true;
    }
    public void ResetTrigger(){
        anim.ResetTrigger("Open");
    }
    public bool GetIsOverviewPanel(){
        return isOverviewPanel;
    }
}
