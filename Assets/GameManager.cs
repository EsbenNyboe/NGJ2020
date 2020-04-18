using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static bool godMode;
    public bool godModeEnabled;
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

}
