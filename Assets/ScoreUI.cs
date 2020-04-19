using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI text;
    GameManager gameManager;
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText( "Score: " + gameManager.score.ToString());
        
    }
}
