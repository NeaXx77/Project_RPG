using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerFollowing : MonoBehaviour
{
    public GameObject[] characters;
    public Vector3 offset;
    public GameManage manager;
    private void Awake() {
        manager = FindObjectOfType<GameManage>();
    }

    void LateUpdate()
    {
        //Make the camera follow the player
        if(manager.currentCharacter == GameManage.character.Clyde){
            transform.position = characters[0].transform.position + offset;
        }
        else
        {
            transform.position = characters[1].transform.position + offset;
        }
    }
}
