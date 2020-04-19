using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRoot : MonoBehaviour
{

    public Transform fly;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(fly.position - Vector3.up,Vector3.up);
    }
}
