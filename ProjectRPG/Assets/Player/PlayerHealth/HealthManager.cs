using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth,currentHealth;
    public Image healthBar;
    public enum ElementDmg{
        fire,
        ice,
        toxic
    }
    void Awake(){
        healthUpdate();
    }
    void healthUpdate(){
        if(currentHealth <= 0){
            currentHealth = 0;
        }
        //healthBar.fillAmount = (currentHealth/maxHealth);
    }
    public void TakeDamage(int amount){
        currentHealth -= amount;
        healthUpdate();
    }
    public void ElementalDamage(int amount, ElementDmg element){}
}
