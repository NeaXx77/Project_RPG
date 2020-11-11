using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ArrowScript : MonoBehaviour
{
    Vector2 angle,veloc;
    Vector2 hitOffset;
    bool hasCollide = false,isShooted = false;
    public float speed;
    public float timer,gravity;
    Transform objectHit;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject,8);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        veloc = new Vector2(rb.velocity.x,rb.velocity.y);
        if(!hasCollide && angle != Vector2.zero && !isShooted){
            rb.AddForce(new Vector2((angle.x*speed),(angle.y*speed)));
            isShooted = true;
        }else if(hasCollide){
            transform.position = (Vector2)objectHit.position - hitOffset;
        }
        if(timer <= 0 && !hasCollide){
            rb.gravityScale = gravity;
            getArrowAngle();

        }else{
            timer -= Time.deltaTime;
            rb.gravityScale = 0;
        }
    }
    public void setAngle(Vector2 angleJoy){
        angle = angleJoy;
    }
    public void OnTriggerEnter2D(Collider2D other) {
        if(!hasCollide){
            GameObject obj = other.gameObject;
            if(obj.CompareTag("Player") || obj.CompareTag("Projectiles") || obj.layer == 12){
                return;
            }
            GetComponent<Rigidbody2D>().isKinematic = true;
            hasCollide = true;
            objectHit = other.gameObject.transform;
            hitOffset = new Vector2(objectHit.position.x - transform.position.x, objectHit.position.y - transform.position.y);
            Destroy(gameObject,2);
        }
    }
    void getArrowAngle(){
        float inputAngle = Mathf.Atan2(veloc.x,veloc.y);
        transform.rotation = Quaternion.Euler(0,0, -(inputAngle * Mathf.Rad2Deg)+90);
    }
}
