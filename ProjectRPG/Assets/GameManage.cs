using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManage : MonoBehaviour
{
    public int lootInventoryPlaces;
    public GameObject[] player;
    public GameObject inventoryObject;
    public InventoryUI inventoryUI;
    public float SWITCHCOOLDOWNTIME, switchCooldown;
    public enum character{
        Clyde,
        Aylei
    }
    public character currentCharacter;
    private void Awake() {
        inventoryObject.SetActive(false);
    }
    public void ChangeActiveInventory(int inventoryFocus){
        switch (inventoryFocus)
        {
            case 0:
                inventoryUI.potionInventory.SetActive(true);
                inventoryUI.lootInventory.SetActive(false);
            break;
            case 1:
                inventoryUI.potionInventory.SetActive(false);
                inventoryUI.lootInventory.SetActive(true);
            break;
        }
    }
    /* mettre l'anim d'entrée en entry dans l'animator
    sur le perso désactivé/sortant: -désactiver tous move et colliders
    -Si vie partagée , gérer la vie dans un objet en dehors des persos
    1) Setactive l'autre perso
    2) Lancer l'anim de sortie du perso
    3) var animator isSwitching (sur le perso entrant) pour lancer l'anim d'entrée 
    (voir si après le setactive si on peut ne pas afficher tout de suite le perso(sprite renderer desactivé) pour que l'anim voulu soit déjà lancée)
    4)desactiver les colliders (invincible jusqu'à l'attérissage ou la fin de l'animation(désactiver de base lorsque le perso est désactivé))
    5)Activer les colliders
    6)possibilitée d'attaquer et se déplacer

     */
    public void SwitchCharacters(int currentCharacter){
        switch (currentCharacter)
        {
            case 0/*Clyde into Aylei*/:
                this.currentCharacter = character.Aylei;
                disableCharacter(player[0]);
                enableCharacter(player[1]);
                break;
            case 1/*Aylei into Clyde*/:
                this.currentCharacter = character.Clyde;
                disableCharacter(player[1]);
                enableCharacter(player[0]);
                break;
        }
    }
    public void disableCharacter(GameObject character){
        character.GetComponent<SpriteRenderer>().enabled = false;
        character.SetActive(false);
    }
    public void enableCharacter(GameObject character){
        character.SetActive(true);
        character.GetComponent<SpriteRenderer>().enabled = true;
    }
}
