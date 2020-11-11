using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewScript : MonoBehaviour
{
    [SerializeField]FieldOfView fieldOfView;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDir(transform.rotation.eulerAngles.z);
        print(transform.rotation.z);
    }
}
