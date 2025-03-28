//Add this script to an empty gameobject
// add  ui > textmeshpro and put the initial text as "Score: 0" , assign it in the gamemanager , You could also use the font included in /Assets/Fonts

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score = 0;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        instance = this; 
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;
    }
}
