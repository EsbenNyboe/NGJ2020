using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static bool musicIsPlaying;

    AudioSource audioSource;
    // Start is called before the first frame update
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (musicIsPlaying == false)
        {
            audioSource.Play();
            musicIsPlaying = true;
            DontDestroyOnLoad(this);
        }
    }
}