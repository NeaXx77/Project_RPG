using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public float textLatency;
    private Queue<string> sentences;
    public TextMeshProUGUI textBox;
    public GameObject TextBoxObject;
    public PlayerInputController playerInput; //pour nextSentence
    //public Sprite textBoxSprite;
    private void Start() {
        sentences = new Queue<string>();
        playerInput = PlayerInputController.instance;
    }
    
    public void startDialogue(Dialogue dial){
        TextBoxObject.SetActive(true);
        sentences.Clear();
        textBox.text = "";
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public bool DisplayNextSentence(){
        if (sentences.Count == 0)
        {
            endDialogue();
            return false;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        print(dialogue.name +": "+ sentence);
        return true;
    }
    private void endDialogue(){
        TextBoxObject.SetActive(false);
        print("End dialogue");
    }
    IEnumerator TypeSentence(string sentence){
        textBox.text = dialogue.name+": ";
        foreach (char letter in sentence.ToCharArray())
        {
            textBox.text += letter;
            yield return new WaitForSeconds(textLatency);
        }
    }
}
