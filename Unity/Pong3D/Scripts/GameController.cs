using UnityEngine;
using TMPro; // Assuming TextMeshPro is used for UI

public class GameController : MonoBehaviour
{
    public GameObject ball;
    public Starter start;
    private BallController ballController; // Reference to the BallController
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

        ballController.stop();
        UpdateUI();
        Time.timeScale = 1f;
        gameStarted = false;

        PlayMusic(menuAudioSource, gameplayAudioSource);

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
        if (sourceToStop != null && sourceToStop.isPlaying)
        {
            sourceToStop.Stop();
        }

        if (sourceToPlay != null && sourceToPlay.clip != null && !sourceToPlay.isPlaying)
        {
            sourceToPlay.Play();
        }
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

        PlayMusic(menuAudioSource, gameplayAudioSource);

        ballController.ResetSpeed();
        if (start != null)
        {
            start.ResetAnimatorState();
        }

        Debug.Log("Returned to Main Menu");
    }

    public void ScoreGoalLeft()
    {
        if (isPaused) return;

        scoreLeft += 1;
        Debug.Log("Left Goal Scored! Current Score: " + scoreLeft);
        UpdateUI();
        PlayGoalSound();
        ballController.IncreaseSpeed(); 
        ResetBallForNewPoint();
    }

    public void ScoreGoalRight()
    {
        if (isPaused) return;

        scoreRight += 1;
        Debug.Log("Right Goal Scored! Current Score: " + scoreRight);
        UpdateUI();
        PlayGoalSound();
        ballController.IncreaseSpeed();
        ResetBallForNewPoint();
    }

    private void UpdateUI()
    {
        scoreLeftText.text = scoreLeft.ToString();
        scoreRightText.text = scoreRight.ToString();
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
        scoreLeft = 0;
        scoreRight = 0;
        UpdateUI();
        ballController.stop();
        ball.transform.position = startingPosition;
        ballController.ResetSpeed(); 
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
        scoreLeft = 0;
        scoreRight = 0;
        UpdateUI();
        ballController.stop();
        ball.transform.position = startingPosition;
        ballController.ResetSpeed();
        start.StartCountdown();
    }
}