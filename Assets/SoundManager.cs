using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioEvent[] collisionVoices;
    public int numCollVoices;
    public int colIndex =-1;
    // Start is called before the first frame update
    void Start()
    {
        collisionVoices = new AudioEvent[numCollVoices];
    }
    public void PlayCollision(AudioEvent audioEvent)
    {
        colIndex++;
        if (colIndex == numCollVoices)
            colIndex = 0;

        collisionVoices[colIndex] = audioEvent;
        if (colIndex == numCollVoices - 1)
            collisionVoices[0].StopSound();
        else if(collisionVoices[colIndex+1] != null)
            collisionVoices[colIndex + 1].StopSound(); 
        audioEvent.PlaySound();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
