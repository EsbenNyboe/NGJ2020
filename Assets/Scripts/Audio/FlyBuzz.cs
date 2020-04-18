using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBuzz : MonoBehaviour
{
    AudioSource audioSource;
    Vector3 lerpin;
    Vector3 vel;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        vel = FlyController2.flyVelocity;

        audioSource.pitch = 1 + lerpin.y;
        
        lerpin = Vector3.Lerp(lerpin, vel, 0.001f);
        Debug.Log("Lerp: " + lerpin.y);
//        Debug.Log("speed: " + FlyController2.speed);
        Debug.Log("velx: " + vel.x);
        Debug.Log("vely: " + vel.y);
        Debug.Log("velz: " + vel.z);
    }
}
