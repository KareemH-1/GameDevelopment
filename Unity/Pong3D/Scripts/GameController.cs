using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject ball;
    public Starter start;
    private BallController ballController;
    public TextMeshProUGUI scoreLeftText, scoreRightText;
    private int scoreLeft = 0, scoreRight = 0;
    private Vector3 startingPosition;

    public AudioClip goalSoundClip;
    private AudioSource audioSourceGoal;

    public AudioClip startSoundClip;
    private AudioSource audioSourceStart;

    public AudioClip gameplayMusicClip;
    public AudioClip menuMusicClip;

    private AudioSource gameplayAudioSource;
    private AudioSource menuAudioSource;

    public GameObject gameModePanel;
    public RacketController leftRacket;
    public RacketController rightRacket;

    public GameObject pauseMenuPanel;
    private bool isPaused = false;
    private bool gameStarted = false;

    private bool wasBallMovingBeforePause = false;

    private bool doPlayMusic = true;
    public TextMeshProUGUI musicStatusText;
    public TextMeshProUGUI musicStatusTextPause;

    private int targetScore = -1;
    private bool isInfiniteScore = true;
    private string currentGameMode;
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;
    public Button returnToMainMenuButton;

    void Start()
    {
        ballController = ball.GetComponent<BallController>();
        startingPosition = ball.transform.position;

        audioSourceGoal = gameObject.AddComponent<AudioSource>();
        audioSourceGoal.playOnAwake = false;

        audioSourceStart = gameObject.AddComponent<AudioSource>();
        audioSourceStart.playOnAwake = false;

        gameplayAudioSource = gameObject.AddComponent<AudioSource>();
        gameplayAudioSource.loop = true;
        gameplayAudioSource.playOnAwake = false;
        gameplayAudioSource.clip = gameplayMusicClip;

        menuAudioSource = gameObject.AddComponent<AudioSource>();
        menuAudioSource.loop = true;
        menuAudioSource.playOnAwake = false;
        menuAudioSource.clip = menuMusicClip;

        if (gameModePanel != null)
        {
            gameModePanel.SetActive(true);
        }
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (returnToMainMenuButton != null)
        {
            returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }

        ballController.stop();
        UpdateUI();
        Time.timeScale = 1f;
        gameStarted = false;

        PlayMusic(menuAudioSource, gameplayAudioSource);
        UpdateMusicStatusText();

        ballController.ResetSpeed();
        if (start != null)
        {
            start.ResetAnimatorState();
        }
    }

    void Update()
    {
        if (gameStarted && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PlayMusic(AudioSource sourceToPlay, AudioSource sourceToStop)
    {
        if (doPlayMusic == false)
        {
            if (sourceToStop != null && sourceToStop.isPlaying) sourceToStop.Stop();
            if (sourceToPlay != null && sourceToPlay.isPlaying) sourceToPlay.Stop();
            UpdateMusicStatusText();
            return;
        }

        if (sourceToStop != null && sourceToStop.isPlaying)
        {
            sourceToStop.Stop();
        }

        if (sourceToPlay != null && sourceToPlay.clip != null && !sourceToPlay.isPlaying)
        {
            sourceToPlay.Play();
        }
        UpdateMusicStatusText();
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        wasBallMovingBeforePause = !ballController.stopped;

        ballController.stop();
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        PlayMusic(menuAudioSource, gameplayAudioSource);
        UpdateMusicStatusText();
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (wasBallMovingBeforePause && gameStarted)
        {
            ballController.EnableMovement();
            PlayMusic(gameplayAudioSource, menuAudioSource);
        }
        else
        {
            PlayMusic(menuAudioSource, gameplayAudioSource);
        }
        UpdateMusicStatusText();
        Debug.Log("Game Resumed");
    }

    public void ReturnToMainMenu()
    {
        scoreLeft = 0;
        scoreRight = 0;
        UpdateUI();

        ballController.stop();
        ball.transform.position = startingPosition;

        Time.timeScale = 1f;
        isPaused = false;
        gameStarted = false;
        wasBallMovingBeforePause = false;

        if (gameModePanel != null)
        {
            gameModePanel.SetActive(true);
        }
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        PlayMusic(menuAudioSource, gameplayAudioSource);
        UpdateMusicStatusText();
        ballController.ResetSpeed();
        if (start != null)
        {
            start.ResetAnimatorState();
        }

        Debug.Log("Returned to Main Menu");
    }

    private void UpdateMusicStatusText()
    {
        if (musicStatusText != null)
        {
            bool musicIsPlayingCurrently = (menuAudioSource != null && menuAudioSource.isPlaying) ||
                                            (gameplayAudioSource != null && gameplayAudioSource.isPlaying);

            string statusText = "Music: Off";
            if (doPlayMusic && musicIsPlayingCurrently)
            {
                statusText = "Music: On";
            }

            if (musicStatusText != null)
            {
                musicStatusText.text = statusText;
            }
            if (musicStatusTextPause != null)
            {
                musicStatusTextPause.text = statusText;
            }
        }
    }

    public void ScoreGoalLeft()
    {
        if (isPaused) return;

        scoreRight += 1;
        Debug.Log("Left Goal Scored! Score on Left UI (Player's score): " + scoreRight);

        UpdateUI();
        PlayGoalSound();
        ballController.IncreaseSpeed();
        CheckWinCondition();
        if (!gameStarted) return;
        ResetBallForNewPoint();
    }

    public void ScoreGoalRight()
    {
        if (isPaused) return;

        scoreLeft += 1;
        Debug.Log("Right Goal Scored! Score on Right UI (AI's score): " + scoreLeft);

        UpdateUI();
        PlayGoalSound();
        ballController.IncreaseSpeed();
        CheckWinCondition();
        if (!gameStarted) return;
        ResetBallForNewPoint();
    }

    private void UpdateUI()
    {
        if (scoreLeftText != null)
        {
            scoreLeftText.text = scoreRight.ToString();
            scoreLeftText.color = Color.white;
        }
        if (scoreRightText != null)
        {
            scoreRightText.text = scoreLeft.ToString();
            scoreRightText.color = Color.white;
        }
    }

    private void PlayGoalSound()
    {
        if (goalSoundClip != null && audioSourceGoal != null)
        {
            audioSourceGoal.PlayOneShot(goalSoundClip);
        }
    }

    private void ResetBallForNewPoint()
    {
        ballController.stop();
        ball.transform.position = startingPosition;
        start.StartCountdown();
    }

    public void StartGame()
    {
        gameStarted = true;
        if (startSoundClip != null && audioSourceStart != null)
        {
            audioSourceStart.PlayOneShot(startSoundClip);
        }
        ballController.StartNewPoint();

        PlayMusic(gameplayAudioSource, menuAudioSource);
        ResumeGame();
    }

    public void StartPlayerVsPlayerMode()
    {
        if (gameModePanel != null)
        {
            gameModePanel.SetActive(false);
        }

        if (leftRacket != null)
        {
            leftRacket.isPlayer = true;
        }
        if (rightRacket != null)
        {
            rightRacket.isPlayer = true;
        }
        currentGameMode = "PvP";
        ResetGameState();
        start.StartCountdown();
    }

    public void StartPlayerVsAIMode()
    {
        if (gameModePanel != null)
        {
            gameModePanel.SetActive(false);
        }

        if (leftRacket != null)
        {
            leftRacket.isPlayer = true;
        }
        if (rightRacket != null)
        {
            rightRacket.isPlayer = false;
        }
        currentGameMode = "PvAI";
        ResetGameState();
        start.StartCountdown();
    }

    private void ResetGameState()
    {
        scoreLeft = 0;
        scoreRight = 0;
        UpdateUI();
        ballController.stop();
        ball.transform.position = startingPosition;
        ballController.ResetSpeed();
    }

    public void changeMusicMode()
    {
        doPlayMusic = !doPlayMusic;
        if (doPlayMusic == false)
        {
            if (gameplayAudioSource != null && gameplayAudioSource.isPlaying)
            {
                gameplayAudioSource.Stop();
            }
            if (menuAudioSource != null && menuAudioSource.isPlaying)
            {
                menuAudioSource.Stop();
            }
        }
        else
        {
            if (gameStarted)
            {
                PlayMusic(gameplayAudioSource, menuAudioSource);
            }
            else
            {
                PlayMusic(menuAudioSource, gameplayAudioSource);
            }
        }
        UpdateMusicStatusText();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetTargetScore(string scoreText)
    {
        Debug.Log($"SetTargetScore called. Received: '{scoreText}'");

        if (int.TryParse(scoreText, out int parsedScore) && parsedScore > 0)
        {
            targetScore = parsedScore;
            isInfiniteScore = false;
        }
        else
        {
            targetScore = -1;
            isInfiniteScore = true;
        }
        Debug.Log("Target score set to: " + targetScore + " (Is Infinite: " + isInfiniteScore + ")");
    }

    private void CheckWinCondition()
    {
        if (isInfiniteScore || !gameStarted) return;

        if (scoreLeft >= targetScore)
        {
            EndGame("AI");
        }
        else if (scoreRight >= targetScore)
        {
            EndGame("Player");
        }
    }

    private void EndGame(string winnerIdentifier)
    {
        gameStarted = false;
        ballController.stop();

        string winnerMessage = "";
        Color winnerColor = Color.green;
        Color loserColor = Color.red;

        if (currentGameMode == "PvP")
        {
            if (winnerIdentifier == "Left")
            {
                winnerMessage = "Left Won!";
                scoreRightText.color = winnerColor;
                scoreLeftText.color = loserColor;
            }
            else
            {
                winnerMessage = "Right Won!";
                scoreRightText.color = loserColor;
                scoreLeftText.color = winnerColor;
            }
        }
        else
        {
            if (winnerIdentifier == "Player")
            {
                winnerMessage = "You Won!";
                scoreLeftText.color = Color.green;
                scoreRightText.color = Color.red;
            }
            else
            {
                winnerMessage = "You Lost!";
                scoreLeftText.color = Color.red;
                scoreRightText.color = Color.green;
            }
        }

        winnerText.text = winnerMessage;
        gameOverPanel.SetActive(true);
    }
}