using UnityEngine;

public class StaminaSounds : MonoBehaviour
{
    private AudioSource audioSource;

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
    public Sound staminaDepleted;
    public Sound staminaRecharging;

    private bool alertSoundIsTimedOut;
    private bool rechargingSoundIsTimedOut;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (FlyController2.grounded == false && FlyController2.currentStamina == 0)
        {
            audioSource.clip = staminaDepleted.soundFile;
        }

        if (FlyController2.grounded == false && FlyController2.currentStamina < 0.2f && alertSoundIsTimedOut == false)
        {
            alertSoundIsTimedOut = true;
            audioSource.clip = staminaLow.soundFile;
        }

        if (FlyController2.grounded == true && FlyController2.currentStamina < 1 && rechargingSoundIsTimedOut == false)
        {
            rechargingSoundIsTimedOut = true;
            audioSource.clip = staminaRecharging.soundFile;
        }
    }
}
