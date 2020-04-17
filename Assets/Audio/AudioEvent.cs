using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEvent : MonoBehaviour
{
    private AudioSource voice;
    private int selectedFile;

    [System.Serializable]
    public class Sound
    {
        public AudioClip soundFile;
        public bool enableParameters;
        [MinMaxRange(0.5f, 2)]
        public RangedFloat pitch;
        public float volume;
    }
    public Sound[] sound;

    private void Start()
    {
        voice = GetComponent<AudioSource>();
    }

    public void PlayTheSound()
    {
        selectedFile = Random.Range(0, sound.Length);
        voice.clip = sound[selectedFile].soundFile;

        if (sound[selectedFile].enableParameters == true) 
        {
            voice.pitch = Random.Range(sound[selectedFile].pitch.minValue, sound[selectedFile].pitch.maxValue);
            voice.volume = sound[selectedFile].volume;
        }
        else // replace this with "else: soundFile inherits its parameters from the AudioEvent"
        {
            voice.pitch = 1;
            voice.volume = 1;
        }
        voice.Play();
    }
}
