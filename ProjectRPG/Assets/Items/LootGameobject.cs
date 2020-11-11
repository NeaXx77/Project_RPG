using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LootGameobject : MonoBehaviour
{
    Transform objectPos;
    public Transform playerPos;
    public LayerMask mask;
    Rigidbody2D body;
    Vector2 relative;
    float xPos,yPos,timeup;
    public float speed;
    public GameManage manager;
    public LootsObject loot;
    BoxCollider2D collider;
    bool fullInventory = false;
    private void Start() {
        manager = GameObject.Find("GameManage").GetComponent<GameManage>();
        objectPos = this.gameObject.transform;
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }
    private void Update() {
        if(manager.currentCharacter == GameManage.character.Clyde){
            playerPos = manager.player[0].transform;
        }else{
            playerPos = manager.player[1].transform;
        }
    }
    private void FixedUpdate() {
        if(!Inventory.instance.lootIsFull(loot)){
            fullInventory = false;
            Physics2D.IgnoreLayerCollision(12,10,false);
        }else{
            fullInventory = true;
            Physics2D.IgnoreLayerCollision(12,10,true);
        }
        if(Physics2D.OverlapCircle(objectPos.position,10,mask) && !fullInventory){
            collider.isTrigger = true;
            if(playerPos.position.x > objectPos.position.x){
                xPos = 1;
            }else{
                xPos = -1;
            }
            if(playerPos.position.y > objectPos.position.y){
                yPos = 1;
            }else{
                yPos = -1;
            }
            relative = new Vector2(xPos*speed*timeup, yPos*speed*timeup);
            body.AddForce(relative);
            timeup += Time.deltaTime;
        }else{
            body.velocity = new Vector2(0,body.velocity.y);
            timeup = 0;
            collider.isTrigger = false;
        }
        if(timeup > 5){
            timeup = 5;
        }
        if(Physics2D.OverlapCircle(objectPos.position,10,mask) && fullInventory){
            print("le slot de l'objet "+loot.lootName+" est plein !");
        }
        Physics2D.IgnoreLayerCollision(12,11);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            if(Inventory.instance.addInventory(loot)){
                Destroy(this.gameObject);
            }
        }
    }
}
