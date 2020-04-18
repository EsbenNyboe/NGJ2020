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
        [MinMaxRange(1, 2)]
        public RangedFloat pitch;
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

//        PlayAlertSound();
        PlayDepletionSound();
        PlayRechargingSound();
        SwitchingStatesSmoothlyRechargingSound();
    }

    private void PlayAlertSound()
    {
        if (FlyController2.grounded == false && FlyController2.currentStamina < 0.4f && voiceA.isPlaying == false && timeOuts[0] > 3)
        {
            voiceA.clip = staminaLow.soundFile;
            voiceA.Play();
        }
    }

    private void PlayDepletionSound()
    {
        if (FlyController2.grounded == false && FlyController2.currentStamina == 0 && voiceB.isPlaying == false)
        {
            voiceA.Stop();
            voiceB.clip = staminaDepleted.soundFile;
            voiceB.Play();
        }
    }

    private void PlayRechargingSound()
    {
        if (FlyController2.grounded == true && FlyController2.currentStamina < 0.9f && voiceC.isPlaying == false)
        {
            voiceB.Stop();
            voiceC.clip = staminaRecharging.soundFile;
            voiceC.Play();
        }
    }

    private void SwitchingStatesSmoothlyRechargingSound()
    {
        if (voiceC.volume < staminaRecharging.volume && FlyController2.grounded == true && FlyController2.currentStamina < 0.9f)
        {
            voiceC.volume += fadeSlope;
        }
        if (voiceC.volume > 0 && (FlyController2.grounded == false || FlyController2.currentStamina > 0.9f))
        {
            voiceC.volume -= fadeSlope;
        }
    }

    private void Play(AudioSource voice, int i, AudioClip clip)
    {
        timeOuts[i] = 0;
        voice.clip = clip;
        voice.Play();
    }
}
