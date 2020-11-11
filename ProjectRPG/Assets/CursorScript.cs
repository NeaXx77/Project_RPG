using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorScript : MonoBehaviour
{
    Mouse currentMouse;
    private void Awake() {
        currentMouse = Mouse.current;
    }
    void LateUpdate()
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(currentMouse.position.ReadValue());
    }
}
