using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;

    
    public TMP_Text scoreText;

    private void Awake()
    {
        if (FindObjectsOfType<ScoreManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        
        if (scoreText != null)
        {
            Debug.Log("updating the score to: " + score.ToString());
            scoreText.text = "Score: " + score.ToString();

        }
    }
}


