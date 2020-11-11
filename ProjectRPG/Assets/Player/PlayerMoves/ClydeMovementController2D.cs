using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(CapsuleCollider2D))]
//sprint/run left et sprint/run in other side animation problem
public class ClydeMovementController2D : MonoBehaviour
{
    #region raycast 
        [SerializeField]
        private Transform headRayOrigin; //The Transform component for the head position of the player
        public float raydistance; //Ray distance from the left/right border of the head
        public LayerMask rayMaskWhatIsWall; //Layer mask for what is wall
    #endregion
    #region unityComponents
        public Rigidbody2D playerRb{get;private set;} //Rigidbody of the player
        public LayerMask whatisGround, crouchLayers;
        [SerializeField]
        private Transform groundcheck;
        private Transform playerTransform;
        private CapsuleCollider2D playerCol;
        public PlayerInputController inputController;

        #region Vector
            Vector2 colliderNotCrouchOffset,colliderNotCrouchSize;
            Vector2 jumpVec;
        #endregion
    #endregion
    #region variables
        public int jumpForce,wallSlideSpeed, wallJumpForce;
        private byte jumpcount, jumpOnWallCount = 1;
        public float speedAxis,speedSprintRatio;
        [SerializeField]
        private float recoverSlide,jumpscale,slideForce;
        [SerializeField]
        float timerWallOut, timerWallStay, timerSlide, tempoGrounded = 0; //Timers - Timer to go off the wall when pressing the opposite dir | Time before the player release the wall
        float speedScale = 1,speedOffset = 3, slidespeed;
        [SerializeField]
        public bool isGrounded,isOnWall = false;
        public bool isMoving = false, crouch = false,crouchCeilling,isSprinting = false,holdingJump = false,JumpWasReleased = true;
        private bool rightRayNotNull = false, leftRayNotNull=false, fromWall = false;
        [HideInInspector]
        public bool isHoldingRight,isHoldingLeft, facingRight, wasRight, isSliding, isFalling;
        
    #endregion
    private void Awake(){
        playerCol = GetComponent<CapsuleCollider2D>();                              //Get the collider of the player
        colliderNotCrouchOffset = playerCol.offset;                                 //Get initial data of the collider to switch manually with crouch position
        colliderNotCrouchSize = playerCol.size;                                     
        //inputController = GetComponent<PlayerInputController>();
        playerRb = GetComponent<Rigidbody2D>();                                     //Get the Rigidbody2D of the player
        playerTransform = transform;
        //useless for the moment
        //Check which character is selected
        if(inputController.currentCharacter == 0){
            //Get the right reference of transforms
            groundcheck = GameObject.Find("ClydeGroundCheck").transform;
            headRayOrigin = GameObject.Find("ClydeHeadRayOrigin").transform;
        }else{
            groundcheck = GameObject.Find("AyleiGroundCheck").transform;
            headRayOrigin = GameObject.Find("AyleiHeadRayOrigin").transform;
        }
    }
    void Start()
    {
        timerSlide = recoverSlide; //init the timer with his value
    }
    void Update(){
        jumpVec = new Vector2(playerRb.velocity.x, jumpForce);                     //To optimize, set the jumpVec x actual x velocity and the jump force 
        SpeedScaleRender();                                                        //Clamp the speed according to the player state
        inputController.playerAnimator.SetBool("isSliding",isSliding);             //Update the value of isSliding on the animator
        inputController.playerSetBool("isOnWall",(leftRayNotNull || rightRayNotNull) && !isGrounded);   //Update the value of isOnWall on the animator
        if(isSliding || timerSlide != recoverSlide){    //Check if the player is sliding and already recovered
            slidingTimer();
        }
    }
    void FixedUpdate()
    {
        groundedChecker();  //Check if the player is grounded
        isPlayerFalling();
        wallJump();
        timerWall();
        SlideMove();        
    }

    /// <summary>
    /// Instruction when we get the jump input once
    /// </summary>
    public void onJumpButton()
    {
        isSliding = false;                  //Deactivate slide
        if((jumpcount > 0 && !isOnWall)){   //Check if we can jump and if we are not on wall
            tempoGrounded = 0.2f;              //Update necessary variable on jump
            isGrounded = false;
            JumpWasReleased = false;
            jumpscale = 1;
            playerRb.velocity = jumpVec;
            jumpcount--;
            inputController.playerSetTrigger("Jump");
        }else if(isOnWall){                                                         //If jump on wall
            fromWall = true;                                                            
            if(isHoldingRight && rightRayNotNull && jumpOnWallCount > 0){           //Jump up on wall if the direction pressed is toward the wall
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce + 10);
                jumpOnWallCount--;
                timerWallStay = 1;
                PlayerSpriteScale(true);
            }else if(isHoldingLeft && leftRayNotNull && jumpOnWallCount > 0){       //Jump up on wall if the direction pressed is toward the wall
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce + 10);
                jumpOnWallCount--;
                timerWallStay = 1;
                PlayerSpriteScale(false);
            }else if((rightRayNotNull && !isHoldingRight) ||(rightRayNotNull && isHoldingLeft) || rightRayNotNull){     //Jump off the wall if the direction pressed is the opposite of the wall
                speedScale = -1;
                playerRb.velocity = new Vector2(playerRb.velocity.x - wallJumpForce, 40);
                PlayerSpriteScale(false);
            }else if((leftRayNotNull && isHoldingRight) || (leftRayNotNull && !isHoldingLeft) || leftRayNotNull){
                speedScale = 1;
                playerRb.velocity = new Vector2(playerRb.velocity.x + wallJumpForce, 40);
                PlayerSpriteScale(true);
            }
            inputController.playerSetTrigger("Jump");
        }
    }

    /// <summary>
    /// Dynamicaly change the height of the jump if the player is holding it
    /// </summary>
    public void jumpHolding(){
        if(!JumpWasReleased && playerRb.velocity.y > 0 && !isOnWall && !inputController.animatorController.isAttacking){
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce*jumpscale);
            jumpscale -= Time.deltaTime*2;
        }
    }

    /// <summary>
    /// On jump button release actions
    /// </summary>
    public void JumpButtonReleaseMove(){
        JumpWasReleased = true;
        jumpscale = 1;
        holdingJump = false;                //Reset jump values
        if(!isFalling && !isGrounded)       //On wall gravity behavior
            playerRb.velocity = new Vector2(playerRb.velocity.x,playerRb.velocity.y/2);
    }

    /// <summary>
    /// Update the variable isFalling, check if the player is falling
    /// </summary>
    public void isPlayerFalling(){
        isFalling = (playerRb.velocity.y < 0 && getTimerWallStay() <= 0 && isOnWall) || (!isOnWall && !isGrounded && playerRb.velocity.y < 0);
    }

    /// <summary>
    /// Check if the player object is grounded around the grounded transform
    /// </summary>
    private void groundedChecker()
    {
        if(tempoGrounded <= 0){                                                             //Wait for a little before checking again if is grounded
            isGrounded = Physics2D.OverlapCircle(groundcheck.position,0.6f,whatisGround);   //Check if we hit a collider with the layer ground on a 0.6 radius
        }else{
            tempoGrounded -= Time.deltaTime;
        }

        if(isGrounded){
            jumpOnWallCount = 1; //Reset some value when is grounded again
            jumpscale = 1;
            fromWall = false;
        }

        if(jumpcount < 2 && isGrounded && !inputController.wasJumpKeyPressed()){           //If we are grounded and jump count is different reset jump count
            jumpcount = 2;
        }else if(!isGrounded && isOnWall){ //Else if we are on wall, set jumpscale to default value
            jumpscale = 1;
        }
    }

    /// <summary>
    /// Move the player according to the direction
    /// </summary>
    /// <param name="right">the direction to move on</param>
    public void Move(bool right){
        isMoving = true;
        inputController.playerSetBool("isRunning", true);

        if(!fromWall){                      //Clamp the x velocity according to speed scale if we didn't jump from a wall
            if(isSprinting){
                playerRb.velocity = new Vector2(speedAxis * speedScale, playerRb.velocity.y);
            }else if(crouch){
                playerRb.velocity = new Vector2(speedAxis/2 * (facingRight?(1):(-1)), playerRb.velocity.y);
            }else{
                playerRb.velocity = new Vector2(speedAxis * speedScale, playerRb.velocity.y);
            }
            //Update the sprite direction according to the actual speed
            if(speedScale > 0){
                PlayerSpriteScale(true);    
            }else if(speedScale < 0){
                PlayerSpriteScale(false);
            }
        }else if(isMoving && !isGrounded){          //On air control
            if(right){
                playerRb.velocity = new Vector2(playerRb.velocity.x + Time.deltaTime*40, playerRb.velocity.y);
            }else if(!right){
                playerRb.velocity = new Vector2(playerRb.velocity.x - Time.deltaTime*40, playerRb.velocity.y);
            }
        }

        if(right && !isOnWall){
            if(isGrounded)
                speedScale += Time.deltaTime*8;
            else
                speedScale += Time.deltaTime*6;
        }else if(!right && !isOnWall){
            if(isGrounded)
                speedScale -= Time.deltaTime*8;
            else
                speedScale -= Time.deltaTime*6;
        }
    }

    /// <summary>
    /// On wall behavior
    /// </summary>
    /// <param name="right"></param>
    public void MoveOnWall(bool right){
        isMoving = true;
        inputController.playerSetBool("isRunning", true);
        speedScale = 0;
        if(timerWallStay <= 0){
            timerWallOut = 0;
        }
        if(timerWallOut <= 0){
            Move(right);
        }
        if(playerTransform.localScale.x == 1 && leftRayNotNull && !wasRight){
            PlayerSpriteScale(false);
        }else if(playerTransform.localScale.x == -1 && rightRayNotNull && wasRight){
            PlayerSpriteScale(true);
        }
    }

    /// <summary>
    /// On wall manager, check if we are on wall and control the behavior
    /// on it
    /// </summary>
    private void wallJump(){
        RaycastHit2D[] hit = new RaycastHit2D[2];
        hit[0] = Physics2D.Raycast((Vector2)headRayOrigin.position + Vector2.right*1.5f, Vector2.right*2.4f,raydistance, rayMaskWhatIsWall);
        hit[1] = Physics2D.Raycast((Vector2)headRayOrigin.position + Vector2.left*1.5f, Vector2.left*2.4f,raydistance, rayMaskWhatIsWall);
        Debug.DrawLine((Vector2)headRayOrigin.position + Vector2.right*1.5f, (Vector2)headRayOrigin.position + Vector2.right*1.5f + new Vector2(raydistance,0),Color.red);
        Debug.DrawLine((Vector2)headRayOrigin.position + Vector2.left*1.5f, (Vector2)headRayOrigin.position + Vector2.left*1.5f - new Vector2(raydistance,0),Color.red);

        if(hit[0].collider != null){            //Right ray hitting the wall
            rightRayNotNull = true;
            leftRayNotNull = false;
            if(!isGrounded){
                if(fromWall && !isOnWall){ //If we jump from another wall and we are not currently on a wall
                    if(!wasRight){
                        jumpOnWallCount = 1;
                        //jumpcount = 1;
                    }
                }
                wasRight = true;
            }
        }else if(hit[1].collider != null){      //Left ray hitting the wall
            rightRayNotNull = false;
            leftRayNotNull = true;
            if(!isGrounded){
                if(fromWall && !isOnWall){
                    if(wasRight){
                        jumpOnWallCount = 1;
                        //jumpcount = 1;
                    }
                }
                wasRight = false;
            }
        }else{
            rightRayNotNull = false;
            leftRayNotNull = false;
        }

        if(!isGrounded && ((rightRayNotNull)||(leftRayNotNull)) ){
            isOnWall = true;
            if(timerWallStay <= 0){
                playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y);  //Gravity on wall after some time
                if(isMoving){
                    Move(facingRight);
                }
            }else if(timerWallStay > 0 && playerRb.velocity.y <= 0){
                if((rightRayNotNull && facingRight) || (leftRayNotNull && !facingRight)){
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
                }
            }
        }else{
            isOnWall = false;
        }
        //Update the direction of the sprite according to the action done before
        if(fromWall && wasRight && !isOnWall && !inputController.right){
            PlayerSpriteScale(false);
        }else if(fromWall && !wasRight && !isOnWall && inputController.right){
            PlayerSpriteScale(true);
        }else if(!fromWall && playerTransform.localScale.x == -1 && rightRayNotNull && !isGrounded){
            PlayerSpriteScale(true);
        }else if(!fromWall && playerTransform.localScale.x == 1 && leftRayNotNull && !isGrounded){
            PlayerSpriteScale(false);
        }
    }

    /// <summary>
    /// On input release, behavior of the player when he stop
    /// </summary>
    public void onReleasedAxis(){
        isHoldingLeft = isHoldingRight = false;
        isMoving = false;
        inputController.playerSetBool("isRunning", false);
        if(isGrounded && !inputController.playerAnimator.GetBool("isAttacking")){
            if(facingRight){
                playerRb.velocity = new Vector2(playerRb.velocity.x * 0.3f, playerRb.velocity.y);
            }else{
                playerRb.velocity = new Vector2(playerRb.velocity.x * 0.3f, playerRb.velocity.y);
            }
        }else if(isGrounded && !isSprinting){
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
        }
        if(speedScale != 0){
                if(speedScale > 0){
					speedScale -= Time.deltaTime*4;
				}else if(speedScale < 0){
					speedScale += Time.deltaTime*4;
				}
				if(speedScale <= 0.1f && speedScale >= -0.1f){
					speedScale = 0;
				}
		}
    }

    private void timerWall(){
        if(isOnWall && isMoving && isHoldingLeft && rightRayNotNull){
            timerWallOut -= Time.deltaTime;
        }else if(isOnWall && isMoving && isHoldingRight && leftRayNotNull){
            timerWallOut -= Time.deltaTime;
        }
            
        if(!isOnWall){
            timerWallOut = 0.6f;
        }
        if(isOnWall){
            timerWallStay -= Time.deltaTime;
        }else{
            timerWallStay = 1;
        }
    }
    /// <summary>
    /// Set the right speed when sprinting or not
    /// </summary>
    void SpeedScaleRender(){

        if(isSprinting){                                //Check if the player object is sprinting
            if(speedScale > speedSprintRatio){          //Check in which direction he is sprinting
			    speedScale = speedSprintRatio;          //Set the sprint speed
		    }else if(speedScale < -speedSprintRatio){
                speedScale = -speedSprintRatio;
            }
        }else if(!isSprinting){                         //If he is not sprinting, set the default speed value
            if(speedScale > 1){
			    speedScale = 1;
		    }else if(speedScale < -1){
                speedScale = -1;
            }
        }
        if(!isSprinting)                                                                            //Clamp the speed on air
            if(playerRb.velocity.x < -wallJumpForce - speedOffset && !isGrounded){
                playerRb.velocity = new Vector2(-wallJumpForce - speedOffset,playerRb.velocity.y);
            }else if(playerRb.velocity.x > wallJumpForce + speedOffset && !isGrounded){
                playerRb.velocity = new Vector2(wallJumpForce + speedOffset,playerRb.velocity.y);
            }
        facingRight = inputController.right;            //Get the orientation of the player
	}

    /// <summary>
    /// Set the X component of the scale in the given direction
    /// </summary>
    /// <param name="scaleRight"></param>
    public void PlayerSpriteScale(bool scaleRight){
        if(scaleRight){
            playerTransform.localScale = new Vector2(1, 1);
        }else{
            playerTransform.localScale = new Vector2(-1, 1);
        }
    }

    public float getTimerWallStay(){
        return timerWallStay;
    }

    /// <summary>
    /// Crouch the player if possible, also 
    /// update isSliding if the player is sprinting
    /// </summary>
    public void isCrouch(){
        crouchCeilling = Physics2D.OverlapCircle((Vector2)headRayOrigin.position + Vector2.up*2, 1f, crouchLayers);
        if(isGrounded && !isSprinting && !isSliding){
            crouch = true;
        }else if(!isGrounded){
            crouch = false;
        }
        if(isSprinting && !isSliding && isGrounded && Mathf.Abs(playerRb.velocity.x) > speedAxis-1f && timerSlide == recoverSlide){
            //Slide bool
            inputController.playerSetTrigger("Slide");
            isSliding = true;
        }
    }

    /// <summary>
    /// Check every condition to make the player sprinting and set isSprinting true or false
    /// </summary>
    /// <param name="input"></param>
    public void SprintingMove(bool input){
        if(!crouch && !isOnWall && isMoving && input){
            isSprinting = true;
        }else{
            isSprinting = false;
        }
    }

    /// <summary>
    /// stay crouch if it's not possible to stand up
    /// </summary>
    public void releasedCrouch(){
        crouchCeilling = Physics2D.OverlapCircle((Vector2)headRayOrigin.position + Vector2.up*2, 1f, crouchLayers);
        if((crouch && crouchCeilling)){
            crouch = true;
        }else{
            crouch = false;
            playerCol.offset = colliderNotCrouchOffset;
            playerCol.size = colliderNotCrouchSize;
        }
    }

    /// <summary>
    /// Manage the player sliding time
    /// </summary>
    void slidingTimer(){
        if((isSliding && timerSlide > 0) || timerSlide != recoverSlide){
            timerSlide -= Time.deltaTime;
        }
        if(timerSlide < 0){
            isSliding = false;
            timerSlide = recoverSlide;
        }
    }

    public void setSpeedscale(int value){
        speedScale = value;
    }

    public void SetHoldingLeft(bool hold){
        isHoldingLeft = hold;
        isHoldingRight = !hold;
    }

    public void SetHoldingRight(bool holding){
        isHoldingRight = holding;
        isHoldingLeft = !holding;
    }

    /// <summary>
    /// Make the player slide if isSliding is true
    /// </summary>
    public void SlideMove(){
        if(!isSliding){
            slidespeed = slideForce;
        }
        if(isSliding && slidespeed > 0){
            playerRb.velocity = new Vector2(slidespeed*playerTransform.localScale.x, playerRb.velocity.y);
            slidespeed-=(Time.deltaTime*30);
        }else if(slidespeed <= 0){
            slidespeed = 0;
            isSliding = false;
        }
    }

}
