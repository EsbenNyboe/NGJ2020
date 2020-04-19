using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSound : MonoBehaviour
{
    public static bool firstTime = true;

    AudioSource audioSource;
    // Start is called before the first frame update
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(this);
    }

    public void Update()
    {
        if (GameManager.deathSound)
        {
            audioSource.Play();
            GameManager.deathSound = false;
        }
    }
}