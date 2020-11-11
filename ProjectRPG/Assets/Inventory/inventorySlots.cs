using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class inventorySlots : MonoBehaviour
{
    public Image icon;
    public Text stack;
    private int count;
    public Weapon weapon = null;
    public LootsObject loot = null;
    public Potions potion = null;
    private void Awake() {
        icon = GetComponent<Image>();
        icon.sprite = null;
        stack = GetComponentInChildren<Text>();
        stack.text = "";
    }
    public void addItem(Weapon weapon){
        icon.sprite = weapon.icon;
        this.weapon = weapon;
    }
    public void incrementation(){
        count++;
        stack.text = count.ToString();
    }
    public void addItem(LootsObject loot){
        icon.sprite = loot.itemSprite;
        this.loot = loot;
        count = loot.stack;
        stack.text = count.ToString();
    }

    public void addItem(Potions potion){
        icon.sprite = potion.itemSprite;
        this.potion = potion;
        count = (potion.stack);
        stack.text = count.ToString();
    }
    public void clear(){
        weapon = null;
        loot = null;
        potion = null;;
        icon.sprite = null;
        count = 0;
        stack.text = "";
    }
    public int getCount(){
        return count;
    }
}
