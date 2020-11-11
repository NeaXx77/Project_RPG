using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue : System.Object
{
    public string name;

    [TextArea]
    public string[] sentences;
}
