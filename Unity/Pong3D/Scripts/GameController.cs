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

    public RacketController aiRacket;

    public GameObject aiDifficultyPanel;

    public GameObject scoreSelectionPanel;
    public TMP_InputField scoreInputField;


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

        if (gameModePanel != null) { gameModePanel.SetActive(true); }
        if (pauseMenuPanel != null) { pauseMenuPanel.SetActive(false); }
        if (gameOverPanel != null) { gameOverPanel.SetActive(false); }
        if (aiDifficultyPanel != null) { aiDifficultyPanel.SetActive(false); }
        if (scoreSelectionPanel != null) { scoreSelectionPanel.SetActive(false); }

        if (returnToMainMenuButton != null)
        {
            returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }

        ResetFullGameState();

        PlayMusic(menuAudioSource, gameplayAudioSource);
        UpdateMusicStatusText();

        if (aiRacket == null)
        {
            Debug.LogError("AI Racket reference is not assigned in GameController! Please assign it in the Inspector.");
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
        ResetFullGameState();

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
        if (aiDifficultyPanel != null)
        {
            aiDifficultyPanel.SetActive(false);
        }
        if (scoreSelectionPanel != null)
        {
            scoreSelectionPanel.SetActive(false);
        }

        PlayMusic(menuAudioSource, gameplayAudioSource);
        UpdateMusicStatusText();

        Debug.Log("Returned to Main Menu");
    }

    private void ResetFullGameState()
    {
        scoreLeft = 0;
        scoreRight = 0;
        UpdateUI();

        ballController.stop();
        ball.transform.position = startingPosition;
        ballController.ResetSpeed();

        if (start != null)
        {
            start.ResetAnimatorState();
        }

        Time.timeScale = 1f;
        isPaused = false;
        gameStarted = false;
        wasBallMovingBeforePause = false;

        if (leftRacket != null) { leftRacket.isPlayer = true; }
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
        if (isPaused || !gameStarted) return;

        scoreRight += 1;
        Debug.Log("Left Goal Scored! Score on Left UI (Player's score): " + scoreRight);

        UpdateUI();
        PlayGoalSound();
        ballController.IncreaseSpeed(1f);
        CheckWinCondition();
        if (gameStarted)
        {
            ResetBallForNewPoint();
        }
    }

    public void ScoreGoalRight()
    {
        if (isPaused || !gameStarted) return;

        scoreLeft += 1;
        Debug.Log("Right Goal Scored! Score on Right UI (AI's score): " + scoreLeft);

        UpdateUI();
        PlayGoalSound();
        ballController.IncreaseSpeed(1f);
        CheckWinCondition();
        if (gameStarted)
        {
            ResetBallForNewPoint();
        }
    }

    private void UpdateUI()
    {
        if (scoreLeftText != null)
        {
            scoreLeftText.text = scoreRight.ToString();
        }
        if (scoreRightText != null)
        {
            scoreRightText.text = scoreLeft.ToString();
        }
        scoreLeftText.color = Color.white;
        scoreRightText.color = Color.white;
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

    public void OpenAIDifficultyPanel()
    {
        if (gameModePanel != null)
        {
            gameModePanel.SetActive(false);
        }
        if (aiDifficultyPanel != null)
        {
            aiDifficultyPanel.SetActive(true);
        }
        Debug.Log("AI Difficulty Panel Opened.");
    }

    public void StartPlayerVsPlayerMode()
    {
        if (gameModePanel != null)
        {
            gameModePanel.SetActive(false);
        }

        if (leftRacket != null) { leftRacket.isPlayer = true; }
        if (rightRacket != null) { rightRacket.isPlayer = true; }

        currentGameMode = "PvP";
        ResetFullGameState();

        if (scoreSelectionPanel != null)
        {
            scoreSelectionPanel.SetActive(true);
        }
        if (scoreInputField != null)
        {
            scoreInputField.text = "";
        }

        Debug.Log("Player Vs Player mode selected. Showing score selection.");
    }

    public void StartPlayerVsAIModeWithDifficulty(string difficulty)
    {
        if (aiDifficultyPanel != null)
        {
            aiDifficultyPanel.SetActive(false);
        }
        if (gameModePanel != null)
        {
            gameModePanel.SetActive(false);
        }

        if (leftRacket != null) { leftRacket.isPlayer = true; }
        if (rightRacket != null) { rightRacket.isPlayer = false; }

        if (aiRacket != null)
        {
            aiRacket.SetAIDifficulty(difficulty);
        }
        else
        {
            Debug.LogError("AI Racket reference not set in GameController for difficulty setting!");
        }

        currentGameMode = "PvAI";
        ResetFullGameState();

        if (scoreSelectionPanel != null)
        {
            scoreSelectionPanel.SetActive(true);
        }
        if (scoreInputField != null)
        {
            scoreInputField.text = "";
        }

        Debug.Log($"Player vs AI game selected with difficulty: {difficulty}. Showing score selection.");
    }

    public void ConfirmScoreAndStartGame()
    {
        string scoreText = scoreInputField.text;
        SetTargetScore(scoreText);

        if (scoreSelectionPanel != null)
        {
            scoreSelectionPanel.SetActive(false);
        }

        start.StartCountdown();
        Debug.Log($"Starting game with custom target score: {(isInfiniteScore ? "Infinite" : targetScore.ToString())}");
    }

    public void SetInputAndConfirmScore(string scoreValue)
    {
        if (scoreInputField != null)
        {
            scoreInputField.text = scoreValue;
        }
        ConfirmScoreAndStartGame();
    }

    public void ReturnToMainMenuFromScoreSelection()
    {
        ReturnToMainMenu();
        if (scoreInputField != null)
        {
            scoreInputField.text = "";
        }
        Debug.Log("Returning to Main Menu from Score Selection.");
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

        string winner = "";
        if (scoreRight >= targetScore)
        {
            winner = (currentGameMode == "PvP") ? "Right" : "Player";
        }
        else if (scoreLeft >= targetScore)
        {
            winner = (currentGameMode == "PvP") ? "Left" : "AI";
        }

        if (!string.IsNullOrEmpty(winner))
        {
            EndGame(winner);
        }
    }

    private void EndGame(string winnerIdentifier)
    {
        gameStarted = false;
        ballController.stop();
        Time.timeScale = 0f;

        string winnerMessage = "";
        Color winnerColor = Color.green;
        Color loserColor = Color.red;

        if (scoreLeftText != null) scoreLeftText.color = Color.white;
        if (scoreRightText != null) scoreRightText.color = Color.white;


        if (currentGameMode == "PvP")
        {
            if (scoreLeft >= targetScore)
            {
                winnerMessage = "Left Player Won!";
                if (scoreLeftText != null) scoreLeftText.color = loserColor;
                if (scoreRightText != null) scoreRightText.color = winnerColor;
            }
            else if (scoreRight >= targetScore)
            {
                winnerMessage = "Right Player Won!";
                if (scoreLeftText != null) scoreLeftText.color = winnerColor;
                if (scoreRightText != null) scoreRightText.color = loserColor;
            }
            else
            {
                winnerMessage = "Game Ended!";
            }
        }
        else
        {
            if (scoreLeft >= targetScore)
            {
                winnerMessage = "You Won!";
                if (scoreLeftText != null) scoreLeftText.color = loserColor;
                if (scoreRightText != null) scoreRightText.color = winnerColor;
            }
            else if (scoreRight >= targetScore)
            {
                winnerMessage = "You Lost!";
                if (scoreLeftText != null) scoreLeftText.color = winnerColor;
                if (scoreRightText != null) scoreRightText.color = loserColor;
            }
            else
            {
                winnerMessage = "Game Ended!";
            }
        }

        if (winnerText != null)
        {
            winnerText.text = winnerMessage;
        }

        if (pauseMenuPanel != null) { pauseMenuPanel.SetActive(false); }
        if (gameModePanel != null) { gameModePanel.SetActive(false); }
        if (aiDifficultyPanel != null) { aiDifficultyPanel.SetActive(false); }
        if (scoreSelectionPanel != null) { scoreSelectionPanel.SetActive(false); }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        PlayMusic(menuAudioSource, gameplayAudioSource);
    }
}