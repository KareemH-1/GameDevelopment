using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public Button playAgainButton;

    public RacketController aiRacket;

    public GameObject aiDifficultyPanel;

    public GameObject scoreSelectionPanel;
    public TMP_InputField scoreInputField;
    public Button confirmScoreButton;

    public Toggle speedResetToggle;
    private bool resetBallSpeedAfterGoal = true;

    private int _previousTargetScore;
    private bool _previousIsInfiniteScore;
    private bool _previousResetBallSpeedAfterGoal;
    private string _previousGameMode;
    private string _previousAIDifficulty;
    private float _previousInitialBallSpeed;

    public Button slowSpeedButton;
    public Button normalSpeedButton;
    public Button fastSpeedButton;

    public Button score1Button;
    public Button score3Button;
    public Button score5Button;
    public Button score10Button;
    public Button score15Button;
    public Button scoreInfiniteButton;

    public GameObject touchControlsPanelPvAI;
    public GameObject touchControlsPanelPvP;

    void Start()
    {
        ballController = ball.GetComponent<BallController>();
        startingPosition = ball.transform.position;

        audioSourceGoal = gameObject.AddComponent<AudioSource>();
        audioSourceGoal.playOnAwake = false;
        audioSourceGoal.clip = goalSoundClip;

        audioSourceStart = gameObject.AddComponent<AudioSource>();
        audioSourceStart.playOnAwake = false;
        audioSourceStart.clip = startSoundClip;

        gameplayAudioSource = gameObject.AddComponent<AudioSource>();
        gameplayAudioSource.loop = true;
        gameplayAudioSource.playOnAwake = false;
        gameplayAudioSource.clip = gameplayMusicClip;

        menuAudioSource = gameObject.AddComponent<AudioSource>();
        menuAudioSource.loop = true;
        menuAudioSource.playOnAwake = true;
        menuAudioSource.clip = menuMusicClip;

        if (gameModePanel != null) { gameModePanel.SetActive(true); }
        if (pauseMenuPanel != null) { pauseMenuPanel.SetActive(false); }
        if (gameOverPanel != null) { gameOverPanel.SetActive(false); }
        if (aiDifficultyPanel != null) { aiDifficultyPanel.SetActive(false); }
        if (scoreSelectionPanel != null) { scoreSelectionPanel.SetActive(false); }

        if (touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(false); }
        if (touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(false); }


        if (returnToMainMenuButton != null)
        {
            returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
        }

        if (speedResetToggle != null)
        {
            speedResetToggle.onValueChanged.RemoveAllListeners();
            speedResetToggle.onValueChanged.AddListener(SetBallSpeedResetAfterGoal);
        }

        SetBallSpeedResetAfterGoal(true);

        if (slowSpeedButton != null) slowSpeedButton.onClick.AddListener(() => SetInitialBallSpeed(7f));
        if (normalSpeedButton != null) normalSpeedButton.onClick.AddListener(() => SetInitialBallSpeed(11f));
        if (fastSpeedButton != null) fastSpeedButton.onClick.AddListener(() => SetInitialBallSpeed(16f));

        if (confirmScoreButton != null)
        {
            confirmScoreButton.onClick.AddListener(ConfirmScoreAndStartGame);
        }

        if (score1Button != null) score1Button.onClick.AddListener(() => SetTargetScore("1"));
        if (score3Button != null) score3Button.onClick.AddListener(() => SetTargetScore("3"));
        if (score5Button != null) score5Button.onClick.AddListener(() => SetTargetScore("5"));
        if (score10Button != null) score10Button.onClick.AddListener(() => SetTargetScore("10"));
        if (score15Button != null) score15Button.onClick.AddListener(() => SetTargetScore("15"));
        if (scoreInfiniteButton != null) scoreInfiniteButton.onClick.AddListener(() => SetTargetScore("infinite"));

        ResetFullGameState();

        UpdateMusicStatusText();

        PlayMusic(gameplayAudioSource, null);
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
        if (touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(false); }
        if (touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(false); }

        PlayMusic(menuAudioSource, gameplayAudioSource);
        UpdateMusicStatusText();
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

        if (gameStarted)
        {
            if (wasBallMovingBeforePause)
            {
                ballController.EnableMovement();
            }
            if (currentGameMode == "PvAI" && touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(true); }
            else if (currentGameMode == "PvP" && touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(true); }
            PlayMusic(gameplayAudioSource, menuAudioSource);
        }
        else
        {
            if (touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(false); }
            if (touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(false); }
            PlayMusic(menuAudioSource, gameplayAudioSource);
        }
        UpdateMusicStatusText();
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
    }

    private void ResetFullGameState()
    {
        scoreLeft = 0;
        scoreRight = 0;
        UpdateUI();

        ballController.stop();
        ball.transform.position = startingPosition;
        ballController.ResetSpeed();
        SetBallSpeedResetAfterGoal(true);

        if (leftRacket != null) { leftRacket.isPlayer = true; leftRacket.ResetTouchStates(); }
        if (rightRacket != null) { rightRacket.ResetTouchStates(); }

        if (start != null)
        {
            start.ResetAnimatorState();
        }

        Time.timeScale = 1f;
        isPaused = false;
        gameStarted = false;
        wasBallMovingBeforePause = false;
        if (touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(false); }
        if (touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(false); }
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
        if (resetBallSpeedAfterGoal)
        {
            ballController.ResetSpeed();
        }
        if (touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(false); }
        if (touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(false); }

        start.StartCountdown();
    }

    public void StartGame()
    {
        _previousTargetScore = targetScore;
        _previousIsInfiniteScore = isInfiniteScore;
        _previousResetBallSpeedAfterGoal = resetBallSpeedAfterGoal;
        _previousGameMode = currentGameMode;
        if (aiRacket != null)
        {
            _previousAIDifficulty = aiRacket.currentAIDifficulty;
        }
        _previousInitialBallSpeed = ballController.initialSpeed;

        gameStarted = true;
        if (startSoundClip != null && audioSourceStart != null)
        {
            audioSourceStart.PlayOneShot(startSoundClip);
        }
        ballController.StartNewPoint();
        PlayMusic(gameplayAudioSource, menuAudioSource);

        if (currentGameMode == "PvAI" && touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(true); }
        else if (currentGameMode == "PvP" && touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(true); }

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
        if (touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(false); }
        if (touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(false); }
    }

    public void StartPlayerVsPlayerMode()
    {
        if (gameModePanel != null)
        {
            gameModePanel.SetActive(false);
        }

        if (leftRacket != null) { leftRacket.isPlayer = true; leftRacket.ResetTouchStates(); }
        if (rightRacket != null) { rightRacket.isPlayer = true; rightRacket.ResetTouchStates(); }

        currentGameMode = "PvP";
        ResetFullGameState();

        if (scoreSelectionPanel != null)
        {
            scoreSelectionPanel.SetActive(true);
        }

        SetTargetScore("infinite");
        SetInitialBallSpeed(11f);
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

        if (leftRacket != null)
        {
            leftRacket.isPlayer = true;
            leftRacket.ResetTouchStates();
        }

        if (rightRacket != null)
        {
            rightRacket.isPlayer = false;
            rightRacket.ResetTouchStates();
        }

        if (aiRacket != null)
        {
            aiRacket.SetAIDifficulty(difficulty);
        }

        currentGameMode = "PvAI";
        ResetFullGameState();

        if (scoreSelectionPanel != null)
        {
            scoreSelectionPanel.SetActive(true);
        }
        SetTargetScore("infinite");
        SetInitialBallSpeed(11f);
    }

    public void SetInitialBallSpeed(float speed)
    {
        if (ballController != null)
        {
            ballController.SetInitialSpeed(speed);
        }
    }

    public void ConfirmScoreAndStartGame()
    {
        string finalScoreText = scoreInputField.text;
        SetTargetScore(finalScoreText);

        if (scoreSelectionPanel != null)
        {
            scoreSelectionPanel.SetActive(false);
        }

        start.StartCountdown();
    }

    public void ReturnToMainMenuFromScoreSelection()
    {
        ReturnToMainMenu();
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
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetTargetScore(string scoreText)
    {
        if (scoreText.ToLower() == "infinite" || string.IsNullOrEmpty(scoreText))
        {
            targetScore = -1;
            isInfiniteScore = true;
            if (scoreInputField != null && scoreInputField.text != "Infinite")
            {
                scoreInputField.text = "Infinite";
            }
        }
        else if (int.TryParse(scoreText, out int parsedScore) && parsedScore > 0)
        {
            targetScore = parsedScore;
            isInfiniteScore = false;
            if (scoreInputField != null && scoreInputField.text != scoreText)
            {
                scoreInputField.text = scoreText;
            }
        }
        else
        {
            targetScore = -1;
            isInfiniteScore = true;
            if (scoreInputField != null && scoreInputField.text != "Infinite")
            {
                scoreInputField.text = "Infinite";
            }
        }
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

        if (leftRacket != null) leftRacket.ResetTouchStates();
        if (rightRacket != null) rightRacket.ResetTouchStates();

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
                winnerMessage = "You Lost";
                if (scoreLeftText != null) scoreLeftText.color = winnerColor;
                if (scoreRightText != null) scoreRightText.color = loserColor;
            }
            else if (scoreRight >= targetScore)
            {
                winnerMessage = "You Won!";
                if (scoreLeftText != null) scoreLeftText.color = loserColor;
                if (scoreRightText != null) scoreRightText.color = winnerColor;
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
        if (touchControlsPanelPvAI != null) { touchControlsPanelPvAI.SetActive(false); }
        if (touchControlsPanelPvP != null) { touchControlsPanelPvP.SetActive(false); }

        PlayMusic(menuAudioSource, gameplayAudioSource);
    }

    public void SetBallSpeedResetAfterGoal(bool shouldReset)
    {
        resetBallSpeedAfterGoal = shouldReset;

        if (speedResetToggle != null && speedResetToggle.isOn != shouldReset)
        {
            speedResetToggle.isOn = shouldReset;
        }
    }

    public void PlayAgain()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        currentGameMode = _previousGameMode;

        if (currentGameMode == "PvP")
        {
            if (leftRacket != null) { leftRacket.isPlayer = true; }
            if (rightRacket != null) { rightRacket.isPlayer = true; }
        }
        else if (currentGameMode == "PvAI")
        {
            if (leftRacket != null) { leftRacket.isPlayer = true; }
            if (rightRacket != null) { rightRacket.isPlayer = false; }
            if (aiRacket != null && !string.IsNullOrEmpty(_previousAIDifficulty))
            {
                aiRacket.SetAIDifficulty(_previousAIDifficulty);
            }
        }

        targetScore = _previousTargetScore;
        isInfiniteScore = _previousIsInfiniteScore;

        SetBallSpeedResetAfterGoal(_previousResetBallSpeedAfterGoal);

        if (ballController != null)
        {
            ballController.SetInitialSpeed(_previousInitialBallSpeed);
        }

        ResetFullGameState();
        StartGame();
    }
}