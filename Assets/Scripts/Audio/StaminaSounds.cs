using UnityEngine;

public class StaminaSounds : MonoBehaviour
{
    private AudioSource voiceA;
    private AudioSource voiceB;
    private AudioSource voiceC;

    [System.Serializable]
    public class Sound
    {
        public AudioClip soundFile;
        [Range(0.5f, 1.5f)]
        public float pitch;
        [Range(0, 1)]
        public float volume;
    }
    public Sound staminaLow;
    public float staminaLowTO;
    public Sound staminaDepleted;
    public float staminaDepletedTO;
    public Sound staminaRecharging;
    public float staminaRechargingTO;
    public float fadeSlope;

    private float rechargingPitcher;
    public float staminaPitchSmooth;
    public float staminaPitchPower;

    private float[] timeOuts;

    private void Start()
    {
        voiceA = GameObject.Find("voiceA").GetComponent<AudioSource>();
        voiceB = GameObject.Find("voiceB").GetComponent<AudioSource>();
        voiceC = GameObject.Find("voiceC").GetComponent<AudioSource>();
        timeOuts = new float[3];
    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            timeOuts[i] += Time.deltaTime * 10;
        }

        rechargingPitcher = Mathf.Lerp(rechargingPitcher, FlyController3.currentStamina*staminaPitchPower, staminaPitchSmooth);
        voiceC.pitch = staminaRecharging.pitch + rechargingPitcher;

        //        PlayAlertSound();
        PlayDepletionSound();
        PlayRechargingSound();
        SwitchingStatesSmoothlyRechargingSound();
    }

    private void PlayAlertSound()
    {
        if (FlyController3.grounded == false && FlyController3.currentStamina < 0.4f && voiceA.isPlaying == false && timeOuts[0] > 3)
        {
            timeOuts[0] = 0;
            voiceA.pitch = staminaLow.pitch;
            voiceA.volume = staminaLow.volume;
            voiceA.clip = staminaLow.soundFile;
            voiceA.Play();
        }
    }

    private void PlayDepletionSound()
    {
        if (FlyController3.grounded == false && FlyController3.currentStamina == 0 && voiceB.isPlaying == false && timeOuts[1] > 3)
        {
            timeOuts[1] = 0;
            voiceA.Stop();
            voiceB.pitch = staminaDepleted.pitch;
            voiceB.volume = staminaDepleted.volume;
            voiceB.clip = staminaDepleted.soundFile;
            voiceB.Play();
        }
    }

    private void PlayRechargingSound()
    {
        if (FlyController3.grounded == true && FlyController3.currentStamina < 0.9f && voiceC.isPlaying == false)
        {
            voiceB.Stop();
            voiceC.clip = staminaRecharging.soundFile;
            voiceC.Play();
        }
    }

    private void SwitchingStatesSmoothlyRechargingSound()
    {
        if (voiceC.volume < staminaRecharging.volume && FlyController3.grounded == true && FlyController3.currentStamina < 0.9f)
        {
            voiceC.volume += fadeSlope;
        }
        if (voiceC.volume > 0 && (FlyController3.grounded == false || FlyController3.currentStamina > 0.9f))
        {
            voiceC.volume -= fadeSlope;
        }
    }
}
