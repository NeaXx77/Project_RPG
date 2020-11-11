using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClydeComboSystem : MonoBehaviour
{
    
        [SerializeField]
        float timerToAttack, BOWTIMERAFTERRELEASED = 0.2f;
        float timerBowAfterReleased;
        int SwordComboNumber, BowComboNumber;
        bool canAttack = true, hasPressedAttack = false, onAttackAnim = false, bowLoading = false;
        bool isHoldingRangedAttack = false;
        Animator attackAnim;
        PlayerRangedWeapon rangedSript;
        [SerializeField]
        float CHARGINGTIME = 1, chargingTimeLeft;
        
    void Start()
    {
        chargingTimeLeft = CHARGINGTIME;
        rangedSript = GetComponent<PlayerRangedWeapon>();
        attackAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if(timerToAttack <= 0){
            hasPressedAttack = false;
            
            SwordComboNumber=BowComboNumber=0;
        }else{
            timerToAttack -= Time.deltaTime;
        }
        if(!canAttack){
            TimerReduced();
        }
        ChargingTimeLeftCooldown();

        attackAnim.SetInteger("SwordNumber",SwordComboNumber);
        attackAnim.SetInteger("BowNumber", BowComboNumber);
        attackAnim.SetBool("hasPressedAttack", hasPressedAttack);
        attackAnim.SetBool("onAttackAnim", onAttackAnim);
    }

    public void AttackButton(bool swordAttack){
        bool isCrouch = attackAnim.GetBool("Crouch");
        bool isGrounded = attackAnim.GetBool("isGrounded");
        if(canAttack && !attackAnim.GetBool("BowButtonHolding") && isGrounded){
            hasPressedAttack = true;
            timerToAttack = 1;
            GetComponent<ClydeAnimatorController>().isAttacking = true;
            if(swordAttack && SwordComboNumber < 2 && !attackAnim.GetBool("Crouch")){
                SwordComboNumber++;
                swordAttack = false;
            }else if(isCrouch){
                //attack crouch here (nothing)
            }
        }
    }

    public void RangedAttackCharging(){
        if(canAttack){
            GetComponent<ClydeAnimatorController>().isAttacking = true;
            attackAnim.SetBool("BowButtonReleased",false);
            attackAnim.SetBool("BowButtonHolding",true);
            isHoldingRangedAttack = true;
        }
    }

    public bool RangedButtonReleased(){
        print("released wtf");
        if (bowLoading)
        {
            isHoldingRangedAttack = false;
            canAttack = false;
            attackAnim.SetBool("BowButtonReleased",true);
            attackAnim.SetBool("BowButtonHolding",false);
            //GetComponent<ClydeAnimatorController>().isAttacking = false;
            if(timerBowAfterReleased <= 0){
                timerBowAfterReleased = BOWTIMERAFTERRELEASED;
            }
            bowLoading = false;
            return true;
        }
        return false;
    }

    //Animation clip fonctions//////////////////////////////////////////////////
    public void BowIsLoaded(){
        SwordComboNumber = 0;
        bowLoading = true;
    }

    public void CanAttackReset(){
        GetComponent<ClydeAnimatorController>().isAttacking = true;
        //onAttackAnim = true;
        hasPressedAttack = false;
        
        GetComponent<ClydeAnimatorController>().AnimatorSetBool("PreviousAttackDone",false);
    }

    public void EndAttack(){
        onAttackAnim = false;
        GetComponent<ClydeAnimatorController>().isAttacking = false;
        
    }

    public void StandingStateReset(){
        GetComponent<ClydeAnimatorController>().isAttacking = false;
    }

    public void EndComboTimerReset(){
        EndAttack();
        SwordComboNumber = 0;
        
    }

    public void SetCanAttack(bool canAttack){
        this.canAttack = canAttack;
    }

    void TimerReduced(){
        if(timerBowAfterReleased > 0)
            timerBowAfterReleased -= Time.deltaTime;
        else
        {
            canAttack = true;
        }
    }

    void ChargingTimeLeftCooldown(){
        if(isHoldingRangedAttack && chargingTimeLeft > 0){
            chargingTimeLeft -= Time.deltaTime;
        }
    }

    public void SetSwordComboNumber(int value){
        SwordComboNumber = value;
    }

    public bool CanAim(){
        bool chargingFinished = chargingTimeLeft <= 0;
        chargingTimeLeft = CHARGINGTIME;
        return (chargingFinished);
    }

    public bool GetCanAttack(){
        return canAttack;
    }

}
