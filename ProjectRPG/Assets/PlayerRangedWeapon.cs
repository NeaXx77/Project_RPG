using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRangedWeapon : MonoBehaviour
{
    public Transform firepoint;
    public PlayerInputController playerInput;
    public GameManage manager;
    private void Awake() {
        playerInput = FindObjectOfType<PlayerInputController>();
        manager = FindObjectOfType<GameManage>();
    }
    public void FireProjectile(GameObject projectile) {
        playerInput.animatorController.isAttacking = false;
        Vector2 rotation;
        rotation = playerInput.getDirValue();
        float inputAngle = Mathf.Atan2(rotation.x,rotation.y);
        Vector2 dir = new Vector2(Mathf.Sin(inputAngle), Mathf.Cos(inputAngle));

        Debug.DrawRay(firepoint.position, rotation*3 ,Color.red,1);
        
        ArrowScript objet = Instantiate(projectile,firepoint.position,Quaternion.Euler(0,0,-(inputAngle * Mathf.Rad2Deg)+90))
                                .GetComponent<ArrowScript>();
        objet.setAngle(dir);
    }
    public Transform getFirepoint(){
        return firepoint;
    }
}
