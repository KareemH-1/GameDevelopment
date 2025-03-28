//Add this script to an empty gameobject
// add  ui > textmeshpro and put the initial text as "Score: 0" , assign it in the gamemanager , You could also use the font included in /Assets/Fonts

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; 
    public TextMeshProUGUI scoreText;
    private int score = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;
    }
    public int GetScore()
    {
        return score;
    }
}
