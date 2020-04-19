using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretch : MonoBehaviour
{
    public Transform end;

    Vector3 offset;

    private void Start()
    {
        offset = end.position - transform.position;
    }

    private void Update()
    {
        transform.position = end.position - (offset)*0.6f;
    }
}
