using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static bool godMode;
    public bool godModeEnabled;
    public List<GameObject> wreckedObjects;
    public int score;

    public void Start()
    {
        wreckedObjects = new List<GameObject>();
        score = 0;
    }
    public void Awake()
    {
     
        if (godModeEnabled)
            godMode = true;
    }
    public static void Death()
    {
        if(!godMode)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
    public bool AddToScore(Collision other)
    {
        bool result = false;
        if (!wreckedObjects.Contains(other.gameObject))
        {
            score++;
            wreckedObjects.Add(other.gameObject);
            result = true;
        }
        return result;
    }

}
