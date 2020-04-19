using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioEvent[] collisionVoices;
    public int numCollVoices;
    public int colIndex =-1;
    public bool firstPlay = true;
    public float cooldownTime;
    // Start is called before the first frame update
    void Start()
    {
        collisionVoices = new AudioEvent[numCollVoices];
    }
    public void PlayCollision(AudioEvent audioEvent)
    {
     //   print("collision sound playing");
       // print(audioEvent.gameObject.name);

        colIndex++;
        if (colIndex == numCollVoices - 1)
            colIndex = 0;

        collisionVoices[colIndex] = audioEvent;
        if (colIndex == numCollVoices - 1)
            collisionVoices[0].StopSound();
        else if(collisionVoices[colIndex+1] != null)
            collisionVoices[colIndex + 1].StopSound();

        if (firstPlay)
        {
            audioEvent.PlaySound(0);
            StartCoroutine(CoolDownCoroutine());
        }
        else
        {
            audioEvent.PlaySound(1);
        }
                

        
    }

    // Update is called once per frame
    void Update()
    {
    }
    bool checkIfNoOneIsPlaying()
    {
        bool noOnePlaying = true; 
        foreach (AudioEvent a in collisionVoices)
        {
            if (a != null)
                if (a.isPlaying())
                {
                    noOnePlaying = false;
                    break;
                }
        }
        return noOnePlaying;
    }
    IEnumerator CoolDownCoroutine()
    {
        firstPlay = false;
        float scale = Time.timeScale;
        yield return new WaitForSeconds(cooldownTime * scale);
        firstPlay = true;
    }
}
