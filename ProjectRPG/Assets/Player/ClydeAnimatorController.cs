using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClydeAnimatorController : MonoBehaviour
{
    private Animator playerAnim;
    PlayerInputController inputAnimator;
    ClydeMovementController2D playerMovement;
    Rigidbody2D playerRigid2D;
    SpriteRenderer spriteRender;
    float comboTimerForInput = 0;
    [SerializeField]
    int runningAttackForce = 60,comboNumber;
    public bool isAttacking = false, waitWhileAttacking = false,sword = false, bow = false,passer;
    bool isGrounded, nextHitCombo = false;
    void Awake(){
        playerAnim = GetComponent<Animator>();
        inputAnimator = GetComponentInParent<PlayerInputController>();
        playerMovement = GetComponent<ClydeMovementController2D>();
        playerRigid2D = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        isGrounded = playerMovement.isGrounded;
        playerAnim.SetBool("isAttacking",isAttacking);
        ActualAnimOnPlay();
        GroundedAnimator();
        SetNoAxisHolding();
    }

    public void ActualAnimOnPlay(){
        inputAnimator.playerSetBool("isGrounded",isGrounded);
        if(!isGrounded){
            inputAnimator.playerSetBool("isOnWall",playerMovement.isOnWall);
        }
        inputAnimator.playerSetBool("isMoving",playerMovement.isMoving);
        notGroundedAnimator();
    }
    public void AttackAnimatorController(){
        if((!playerMovement.isOnWall)){
            if(comboTimerForInput <= 0 && !playerMovement.isSprinting){
                if(sword){
                    inputAnimator.playerSetBool("SwordCombo",true);
                }else if(bow){
                    inputAnimator.playerSetTrigger("BowAttack");
                }
            }
            onAttackButton();
        }
    }
    public void notGroundedAnimator(){
        if(playerMovement.isFalling){
            inputAnimator.playerSetBool("isFalling",true);
        }else{
            inputAnimator.playerSetBool("isFalling",false);
        }
    }
    public void GroundedAnimator(){
        inputAnimator.playerSetBool("Crouch",playerMovement.crouch);
        inputAnimator.playerSetBool("Sprint",playerMovement.isSprinting);
    }
    public void onAttackButton(){
        if(comboTimerForInput > 0 && !nextHitCombo && comboNumber < 2){
            nextHitCombo = true;
            comboNumber++;
        }else if(!isAttacking){
            isAttacking = true;
            comboTimerForInput = 1f;
            if(!playerMovement.isSprinting && playerMovement.isGrounded){
                playerMovement.setSpeedscale(0);
                playerRigid2D.velocity = new Vector2(inputAnimator.right?runningAttackForce:-runningAttackForce,0);
            }
        }
    }
    public void AnimatorSetBool(string name, bool state){
        playerAnim.SetBool(name,state);
    }
    public void setIsAttacking(bool state){
        isAttacking = state;
    }
    bool CheckVelocity(){
        return playerRigid2D.velocity.x == 0;
    }
    void SetNoAxisHolding(){
        AnimatorSetBool("NoAxisHolding",CheckVelocity());
    }
    //Activer trigger avant le sprite renderer pour eviter une frame d'une autre animation que celle d'entrée
    private void OnEnable() {
        spriteRender.enabled = true;
    }
    private void OnDisable() {
        spriteRender.enabled = false;
    }
}
