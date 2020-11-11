using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputController : MonoBehaviour
{
    #region Clyde
    public ClydeMovementController2D playerMove;//Editor
    public ClydeAnimatorController animatorController;//Editor
    public ClydeComboSystem comboScript;//Editor
    public Transform ClydeTransform;
    #endregion Clyde

    public static PlayerInputController instance;
    public Animator playerAnimator;
    Transform firepoint;

    #region Devices
    public Gamepad pad;
    public Keyboard clavier;
    private bool isUsingKeyboard = true,isUsingMouse = true;
    #endregion Devices

    public bool jumpKeyPressed{get;protected set;} = false;
    public bool right = true, arrowUp = false, arrowDown = false;
    private bool waitingForShoot = false;
    private bool sprinting = false;
    public GameManage manager;
    public bool inventoryActive{get;private set;} = false;

    public enum inventoryFocus{
        potion,
        loot
    }
    inventoryFocus inventFocus=inventoryFocus.potion;
    public enum character{
        Clyde,
        Aylei
    }
    public character currentCharacter;
    bool isDialogue = false;
    public InventoryAnimationManager inventoryAnim;
    public InventoryNavManager inventoryNavManager;

    void Awake(){
        manager = FindObjectOfType<GameManage>();
        instance = this;
    }
    void Start(){
        pad = Gamepad.current;
        clavier = Keyboard.current;
    }

    void Update(){
        if(ClydeTransform.localScale.x == 1){
            right = true;
        }else{
            right = false;
        }
        if(manager.currentCharacter == 0){
            ClydeInput();
        }else
        {
            AyleiInput();
        }
        InventoryInput();
    }
    private void FixedUpdate() {
        ClydeMovement();
    }
    void DeviceInUse(){
        //Which device is in use
        if(clavier.anyKey.isPressed){
            isUsingKeyboard = true;
        }else if(pad != null && pad.wasUpdatedThisFrame){
            isUsingKeyboard = false;
        }
        isUsingMouse = isUsingKeyboard;
    }
//Input function keyboard and gamepad for Clyde
    void ClydeInput(){
        if(!isDialogue && !inventoryActive){
            //Jump
            if(JumpButtonPressed()){
                animatorController.setIsAttacking(false);
                playerMove.onJumpButton();
                jumpKeyPressed = true;
            }else if(jumpKeyPressed){
                jumpKeyPressed = false;
            }
            if(JumpButtonReleased()){
                playerMove.JumpButtonReleaseMove();
            }
            if(JumpButtonHolding() ){
                playerMove.jumpHolding();
            }
                //CloseAttack
            if(CloseAttackButtonPressed()){
                    //A modifier selon le perso
                comboScript.AttackButton(true);
            }
            //RangedAttack
            if(RangedAttackButtonPressed() && playerMove.isGrounded){
                comboScript.RangedAttackCharging();
            }else if(RangedAttackButtonReleased() && playerMove.isGrounded && comboScript.GetCanAttack()){
                waitingForShoot = true;
            }
            if(waitingForShoot){
                waitingForShoot = !comboScript.RangedButtonReleased();
            }
            
            //up and down
            if(isUsingKeyboard){
                if(clavier.wKey.isPressed){
                    arrowUp = true;
                    arrowDown = false;
                }else if(clavier.sKey.isPressed){
                    arrowDown = true;
                    arrowUp = false;
                }else{
                    arrowDown = arrowUp = false;
                }
            }
            //crouch control
            if(CrouchButtonPressed()){
                if(!animatorController.isAttacking)
                    playerMove.isCrouch();
                    comboScript.SetSwordComboNumber(0);
                if(playerMove.crouch){
                    animatorController.setIsAttacking(false);
                    if(CloseAttackButtonPressed()){
                        //crouch close attack
                    }else if(RangedAttackButtonPressed()){
                        //crouch rangedattack
                    }
                }
            }else{
                playerMove.releasedCrouch();
            }
            //Bouton de sprint
            if(SprintButtonHolding() && !playerMove.isSliding){
                sprinting = true;
            }else{
                sprinting = false;
            }/*
            if(CanSwitch() && SwitchPlayerButtonPressed()){
                manager.SwitchCharacters((int)currentCharacter);
                manager.switchCooldown = manager.SWITCHCOOLDOWNTIME;
            }*/
        }
    }
    void ClydeMovement(){
        //left and right keyboard and pad function
        if(!animatorController.isAttacking){
            onAxisButton();
            leftJoystickMove();
        }else{
            playerMove.onReleasedAxis();
        }
        if(sprinting){
            animatorController.setIsAttacking(false);
            playerMove.SprintingMove(true);
        }else{
            playerMove.SprintingMove(false);
        }
    }
//Input function keyboard and gamepad for Aylei
    void AyleiInput(){
        //Copie de CLydeInput en changeant les scripts
    }
    void AyleiMovement(){
        //mettre tous les move qui utilise la force, à mettre dans le fixedupdate
    }
    void InventoryInput(){
        //Boutons d'inventaire
        bool inventoryButtonPressed = false;
        if(InventoryButtonPressed()){
            SetActiveInventory();
            inventoryButtonPressed = true;
        }else if(inventoryActive && !inventoryAnim.GetIsOverviewPanel() && NextInventoryTabButtonPressed()){
            ChangeInventoryFocus();
            inventoryButtonPressed = true;
        }
        if(inventoryActive){
            if((pad != null && pad.yButton.wasPressedThisFrame) || clavier.qKey.wasPressedThisFrame){
                inventoryAnim.SwitchInventory();
                inventoryButtonPressed = true;
            }
        }
        if(inventoryButtonPressed){
            inventoryNavManager.OnSwitchPanel();
        }
    }
    public bool wasJumpKeyPressed(){
        return jumpKeyPressed;
    }
//keyboard left "q" and right "d"
    public bool onAxisButton(){
        if(isUsingKeyboard && !playerMove.isSliding){
            if(clavier.aKey.isPressed){
                playerMove.SetHoldingLeft(true);
                if(!playerMove.isOnWall){
                    playerMove.Move(right = false);
                }else{
                    playerMove.MoveOnWall(false);
                }
            }else if(clavier.dKey.isPressed){
                playerMove.SetHoldingRight(true);
                if(!playerMove.isOnWall){
                    playerMove.Move(right = true);
                }else{
                    playerMove.MoveOnWall(true);
                }
            }else{
                playerMove.onReleasedAxis();
            }
        }
        return right;
    }

    public bool leftJoystickMove(){
        if(!isUsingKeyboard && pad != null && !playerMove.isSliding){
            Vector2 padleftJoy = pad.leftStick.ReadValue();
            if(padleftJoy.x < -0.3f){
                playerMove.SetHoldingLeft(true);
                if(!playerMove.isOnWall){
                    playerMove.Move(right = false);
                }else{
                    playerMove.MoveOnWall(false);
                }
            }else if(padleftJoy.x > 0.3f){
                playerMove.SetHoldingRight(true);
                if(!playerMove.isOnWall){
                    playerMove.Move(right = true);
                }else{
                    playerMove.MoveOnWall(true);
                }
            }else{
                playerMove.onReleasedAxis();
            }
        }
        return right;
    }
    //Tester avec manette quand on ne touche pas le joystick x = ? et y = ?
    public Vector2 getDirValue(){
        firepoint = GetComponentInChildren<PlayerRangedWeapon>().getFirepoint();
        
        if(comboScript.CanAim()){
            if(!isUsingKeyboard){
                return pad.leftStick.ReadValue();
            }else if(!isUsingMouse){
                return new Vector2(right?1:-1,(arrowUp?1:arrowDown?-1:0));
            }else{
                //Calcul vecteur entre firepoint et mouseposition
                return -((Vector2)firepoint.position) + (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            }
        }else
        {
            return right?Vector2.right:Vector2.left;
        }
    }

    public void playerSetTrigger(string animName){
            playerAnimator.SetTrigger(animName);
    }

    public void playerSetBool(string anim, bool action){
        playerAnimator.SetBool(anim,action);
    }

    private void SetActiveInventory(){
        if(!inventoryActive){
            manager.inventoryObject.SetActive(inventoryActive = !inventoryActive);
            inventoryAnim.OpenInventory();
        }else
        {
            inventoryAnim.CloseInventory();
            inventoryActive = false;
        }
    }
    
    private void ChangeInventoryFocus(){
        if(inventFocus == inventoryFocus.potion){
            inventFocus = inventoryFocus.loot;
        }else if(inventFocus == inventoryFocus.loot){
            inventFocus = inventoryFocus.potion;
        }
        manager.ChangeActiveInventory((int)inventFocus);
    }

    //input pressed
    public bool JumpButtonPressed(){
        return ((pad != null && pad.aButton.wasPressedThisFrame) || clavier.spaceKey.wasPressedThisFrame);
    }
    public bool JumpButtonReleased(){
        return ((pad != null && pad.aButton.wasReleasedThisFrame) || clavier.spaceKey.wasReleasedThisFrame);
    }
    public bool JumpButtonHolding(){
        return ((pad != null && pad.aButton.isPressed) || clavier.spaceKey.isPressed);
    }
    public bool CrouchButtonPressed(){
        return ((pad != null && pad.bButton.isPressed) || clavier.leftCtrlKey.isPressed);
    }
    public bool SprintButtonHolding(){
        return ((pad != null && pad.leftTrigger.isPressed) || clavier.leftShiftKey.isPressed);
    }
    public bool CloseAttackButtonPressed(){
        return ((pad != null && pad.xButton.wasPressedThisFrame) || clavier.fKey.wasPressedThisFrame);
    }
    public bool RangedAttackButtonPressed(){
        return ((pad != null && pad.yButton.wasPressedThisFrame) || clavier.rKey.wasPressedThisFrame);
    }
    public bool RangedAttackButtonReleased(){
        return ((pad != null && pad.yButton.wasReleasedThisFrame) || clavier.rKey.wasReleasedThisFrame);
    }
    public bool InventoryButtonPressed(){
        return ((pad != null && pad.selectButton.wasPressedThisFrame) || clavier.iKey.wasPressedThisFrame);
    }
    public bool interactionButtonPressed(){
        return ((pad != null && pad.aButton.wasPressedThisFrame) || clavier.eKey.wasPressedThisFrame);
    }
    public bool NextInventoryTabButtonPressed(){
        return ((pad != null && pad.rightShoulder.wasPressedThisFrame) || clavier.tabKey.wasPressedThisFrame);
    }
    public bool SwitchPlayerButtonPressed(){
        return ((pad != null && pad.dpad.up.wasPressedThisFrame) || clavier.qKey.wasPressedThisFrame);
    }
    public bool CanSwitch(){
        //CanSwitch à déplacer dans GameManage ?
        if(manager.switchCooldown > 0){
            manager.switchCooldown -= Time.deltaTime;
            return false;
        }else{
            return true;
        }
    }
    public void setIsDialogue(bool state){
        isDialogue = state;
    }
    public inventoryFocus GetInventoryFocus(){
        return inventFocus;
    }
}
