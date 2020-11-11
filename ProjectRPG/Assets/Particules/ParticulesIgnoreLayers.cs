using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticulesIgnoreLayers : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Physics2D.IgnoreLayerCollision(14,10);
        Physics2D.IgnoreLayerCollision(14,12);
        Physics2D.IgnoreLayerCollision(14,11);
    }
}
