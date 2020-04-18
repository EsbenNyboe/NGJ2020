using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBuzz : MonoBehaviour
{
    AudioSource audioSource;
    float pitch;
    Vector3 vel;
    Vector3 acc;
    Vector3 lerpin;

    float pitchingYspeed;
    float pitchingZspeed;
    float pitchingAccX;
    float pitching;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        vel = FlyController2.flyVelocity;
        acc = FlyController2.flyAcceleration;
        audioSource.pitch = pitch;

        if (audioSource.pitch > 1.4f)
        {
            audioSource.pitch = 1.4f;
        }
        if (audioSource.pitch < 0.7f)
        {
            audioSource.pitch = 0.7f;
        }

        Oldie();
//        NewFailOfTiredness();

    }

    private void NewFailOfTiredness()
    {
        pitchingYspeed = Mathf.Lerp(pitchingYspeed, vel.y, 0.0001f);
        pitch = 1 + pitchingYspeed;
        //UpdatePitching(pitchingYspeed, -1f, 2);
        pitchingZspeed = Mathf.Lerp(pitchingZspeed, vel.z, 0.0001f);
//        pitch = Mathf.Lerp(pitch, pitchingZspeed, 0.);
            //UpdatePitching(pitchingZspeed, -1, 2);
        pitchingAccX = Mathf.Lerp(pitchingAccX, acc.x, 0.005f);
        UpdatePitching(pitchingAccX, -1, 2);


        //        pitch = 1 + pitchingYspeed + pitchingZspeed + pitchingAccX;
    }

    private void UpdatePitching(float pitchingParameter, float pitchMin, float pitchMax)
    {
        pitching = Mathf.Lerp(pitching, pitchingParameter, 0.5f);
       
        if (pitchingParameter > pitchMin && pitchingParameter < pitchMax)
        {
            
        }
    }

    private void Oldie()
    {
        lerpin = Vector3.Lerp(lerpin, vel, 0.0001f);
        pitch = 1 + lerpin.y;
        pitch = Mathf.Lerp(pitch, (acc.x + acc.z), 0.005f);
    }
}
