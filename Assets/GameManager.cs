using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static bool godMode;
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public static void Death()
    {
        if(!godMode)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

}
