using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBuzz : MonoBehaviour
{
    AudioSource audioSource;
    float pitch;
    Vector3 vel;
    Vector3 acc;

    float pitchingVel;
    [Range(1, 10)]
    public float pitchingVelPower;

    float pitchingAcc;
    [Range(1, 10)]
    public float pitchingAccPower;
    
    [MinMaxRange(-0.5f, 0)]
    public RangedFloat resetBottom;
    private float resetBottomChosen;
    [MinMaxRange(0, 0.2f)]
    public RangedFloat resetTop;
    private float resetTopChosen;
    public float resetSlope;
    public float resetTime;
    private float timeSinceReset;

    public float volume;
    public float fadeSlope;

    private bool isGrounded;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timeSinceReset = resetTime;
    }

    void Update()
    {
        timeSinceReset += Time.deltaTime * 10;
        MacroPitching();
        LimitPitchMinMaxValues(0.7f, 1.4f);
        MicroPitching();
        LimitPitchMinMaxValues(0.7f, 1.5f);

        SwitchingStatesSmoothly(); // grounding and ungrounding smoothly
        audioSource.pitch = pitch;

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            StateChangeFlying();
        }
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            StateChangeGrounded();
        }
    }

    private void SwitchingStatesSmoothly()
    {
        if (audioSource.volume > 0 && isGrounded == true)
        {
            audioSource.volume -= fadeSlope;
        }
        if (audioSource.volume < volume && isGrounded == false)
        {
            audioSource.volume += fadeSlope;
        }
    }    

    public void StateChangeFlying() // run this when fly starts flying
    {
        isGrounded = false;
        audioSource.volume = volume;
        ResetPitch();
        //audioSource.UnPause();
    }

    public void StateChangeGrounded() // run this when fly stops flying
    {
        isGrounded = true;
        //audioSource.Pause();
    }

    private void ResetPitch()
    {
        resetBottomChosen = Random.Range(resetBottom.minValue, resetBottom.maxValue);
        resetTopChosen = Random.Range(resetTop.minValue, resetTop.maxValue);
        pitchingVel = resetBottomChosen*Random.Range(0.9f, 1.2f);
        timeSinceReset = 0;
    }

    private void LimitPitchMinMaxValues(float minPitch, float maxPitch)
    {
        if (pitch < minPitch)
        {
            pitch = minPitch;
        }
        if (pitch > maxPitch)
        {
            pitch = maxPitch;
        }
    }

    private void MacroPitching()
    {
        vel = FlyController2.flyVelocity;

        if (timeSinceReset < resetTime)
        {
            float resetAffect = pitchingVelPower * resetSlope * (resetTopChosen - pitchingVel);
            pitchingVel = Mathf.Lerp(pitchingVel, vel.y, resetAffect * 0.00001f); 
        }
        else
        {
            pitchingVel = Mathf.Lerp(pitchingVel, vel.y, pitchingVelPower * 0.00001f);
        }

        pitch = 1 + pitchingVel;
    }

    private void MicroPitching()
    {
        acc = FlyController2.flyAcceleration;
        if (acc.x > acc.y)
        {
            pitchingAcc = acc.x;
        }
        else
        {
            pitchingAcc = acc.y;
        }
        pitch = Mathf.Lerp(pitch, pitchingAcc, pitchingAccPower*0.001f);
    }
}
