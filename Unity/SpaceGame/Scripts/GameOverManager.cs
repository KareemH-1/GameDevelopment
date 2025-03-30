// Shows gameover panel when game is over, stops all playing audios and plays gameover audio.
// Restarts the game if player clicks the restart button or quits when player clicks quit button.
// Also displays highscore and score.
// UI hierarchy: Panel > Finalscore text, Gameover text, Highscore text, Restart button, Quit button.
// Add OnClick events to Restart and Quit buttons to call RestartGame and QuitGame methods.
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public AudioSource gameOverSound;
    public GameObject normalScoreText;
    public GameObject UpgradeText;
    private bool isGameOver = false;

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

    public void GameOver(int score)
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverPanel.SetActive(true);

        if (normalScoreText != null)
        {
            normalScoreText.SetActive(false);
        }
        if (UpgradeText != null)
        {
            UpgradeText.SetActive(false);
        }

        // Stop all sounds
        AudioSource[] allSounds = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource sound in allSounds)
        {
            sound.Stop();
        }

        // Play game over sound once
        if (!gameOverSound.isPlaying)
        {
            gameOverSound.loop = false;
            gameOverSound.Play();
        }

        // Update final score
        finalScoreText.text = "Score: " + score;

        // Update high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        highScoreText.text = "High Score: " + highScore;

        // Stop enemy spawning
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.StopSpawning();
        }
    }

    public void RestartGame()
    {
        if (gameOverSound.isPlaying)
        {
            gameOverSound.Stop();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        if (gameOverSound.isPlaying)
        {
            gameOverSound.Stop();
        }
        PlayerPrefs.Save();
        Application.Quit();
    }
}