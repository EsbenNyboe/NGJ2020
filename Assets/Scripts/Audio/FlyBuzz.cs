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
    [Range(1, 100)]
    public float pitchingVelPower;

    float pitchingAcc;
    public float pitcAccPowUp; // pitchingAccPowerUp
    public float pitcAccPowDown;

    public float pitchingAccMax;

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

    private bool notBuzzing;

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

        Depletion();

        SwitchingStatesSmoothly(); // grounding and ungrounding smoothly
        audioSource.pitch = pitch;

        switch (FlyController3.grounded)
        {
            case true:
                if (notBuzzing == false)
                {
                    StateChangeGrounded();
                }
                break;
            case false:
                if (notBuzzing == true && FlyController3.currentStamina > 0)
                {
                    StateChangeFlying();
                }
                break;
        }
    }

    private void Depletion()
    {
        if (FlyController3.grounded == false && FlyController3.currentStamina == 0)
        {
            notBuzzing = true;
        }
    }

    private void SwitchingStatesSmoothly()
    {
        if (audioSource.volume > 0 && notBuzzing == true)
        {
            audioSource.volume -= fadeSlope;
        }
        if (audioSource.volume < volume && notBuzzing == false)
        {
            audioSource.volume += fadeSlope;
        }
    }    

    public void StateChangeFlying() 
    {
        notBuzzing = false;
        audioSource.volume = volume;
        ResetPitch();
        //audioSource.UnPause();
    }

    public void StateChangeGrounded() 
    {
        notBuzzing = true;
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
        vel = FlyController3.flyVelocity;

        if (timeSinceReset < resetTime)
        {
            float resetAffect = pitchingVelPower * resetSlope * (resetTopChosen - pitchingVel);
            pitchingVel = Mathf.Lerp(pitchingVel, vel.y, resetAffect * 0.00001f); 
        }
        else
        {
            pitchingVel = Mathf.Lerp(pitchingVel, vel.y, pitchingVelPower * 0.00001f);
        }
    }

    private void MicroPitching()
    {
        acc = FlyController3.flyAcceleration;
        float accXY;
        if (acc.x*acc.x > acc.z*acc.z)
        {
            accXY = acc.x;
        }
        else
        {
            accXY = acc.z;
        }

        Debug.Log("accXY:" + accXY);

        if (accXY == 0)
        {
            pitchingAcc = Mathf.Lerp(pitchingAcc, accXY, pitcAccPowDown * 0.01f);
        }
        else
        {
            pitchingAcc = Mathf.Lerp(pitchingAcc, accXY, pitcAccPowUp * 0.001f);
        }

        if (pitchingAcc > pitchingAccMax)
        {
            pitchingAcc = pitchingAccMax;
        }

        pitch = 1 + pitchingVel + pitchingAcc; 
    }

    private void ChoosePositiveValue(ref float acc)
    {
        if (acc < 0)
        {
            acc = acc * -1;
        }
    }

}