using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJinteract : MonoBehaviour
{
    public LayerMask mask;
    private PlayerInputController inputPlayer;
    public bool dialogueStarted = false;
    public DialogueTrigger dialtrig;
    private void Start() {
        inputPlayer = PlayerInputController.instance;//GetComponent<DialogueTrigger>().playerInput;
        dialtrig = GetComponent<DialogueTrigger>();
    }
    void Update()
    {
        if(Physics2D.OverlapCircle(this.gameObject.transform.position,5,mask)){
            if(inputPlayer.interactionButtonPressed()){
                if(!dialogueStarted){
                    print("started");
                    inputPlayer.setIsDialogue(true);
                    dialogueStarted = true;
                    dialtrig.startDialogue(dialtrig.dialogue);
                }else
                {
                    if(!dialtrig.DisplayNextSentence()){
                        dialogueStarted = false;
                        inputPlayer.setIsDialogue(false);
                    }                    
                }
            }
        }
    }
}
