using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpammer : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioEvent[] testEvents;
    int counter = 0;
    SoundManager soundManager;
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayTestSound();
        }
    }
    public void PlayTestSound()
    {
      soundManager.PlayCollision(testEvents[counter]);
        counter++;
        if (counter == testEvents.Length)
            counter = 0;
    }
}
