using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacketController : MonoBehaviour
{
    public float playerSpeed = 10f;
    public KeyCode up;
    public KeyCode down;
    public bool isPlayer = true;
    public float offset = 0.2f;

    public string currentAIDifficulty = "Normal";

    public float easyOffsetMultiplier = 20.0f;
    public float normalOffsetMultiplier = 2.0f;
    public float hardOffsetMultiplier = 0.05f;

    public float easyReactionDelay = 0.5f;
    public float normalReactionDelay = 0.1f;
    public float hardReactionDelay = 0.01f;

    public float easyAISpeed = 5.0f;
    public float normalAISpeed = 10.0f;
    public float hardAISpeed = 15.0f;

    private float _currentOffset;
    private float _currentReactionDelay;
    private float _currentAISpeed;

    private float lastMoveTime;

    public RacketController otherPlayer;
    
    private Rigidbody rb;
    private Transform ball;

    private BallController ballController;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject ballObject = GameObject.FindGameObjectWithTag("Ball");
        if (ballObject != null)
        {
            ball = ballObject.transform;
            ballController = ballObject.GetComponent<BallController>();
        }

        if (ball == null || ballController == null)
        {
            Debug.LogError("Ball GameObject with 'Ball' tag or its BallController not found! AI will not function.");
        }

        SetAIDifficulty(currentAIDifficulty);
    }

    void Update()
    {
        if (isPlayer)
        {
            moveByPlayer();
        }
        else
        {
            moveByAI();
        }
    }

    public int isPlayerOther()
    {
        if(isPlayer == false) return 1;
        else return 2;
    }
    public void SetAIDifficulty(string difficulty)
    {
        currentAIDifficulty = difficulty;
        switch (difficulty)
        {
            case "Easy":
                _currentOffset = offset * easyOffsetMultiplier;
                _currentReactionDelay = easyReactionDelay;
                _currentAISpeed = easyAISpeed;
                break;
            case "Normal":
                _currentOffset = offset * normalOffsetMultiplier;
                _currentReactionDelay = normalReactionDelay;
                _currentAISpeed = normalAISpeed;
                break;
            case "Hard":
                _currentOffset = offset * hardOffsetMultiplier;
                _currentReactionDelay = hardReactionDelay;
                _currentAISpeed = hardAISpeed;
                break;
            default:
                Debug.LogWarning($"Unknown AI difficulty string '{difficulty}' received. Defaulting to Normal.");
                _currentOffset = offset * normalOffsetMultiplier;
                _currentReactionDelay = normalReactionDelay;
                _currentAISpeed = normalAISpeed;
                currentAIDifficulty = "Normal";
                break;
        }
        Debug.Log($"AI Difficulty set to: {currentAIDifficulty} (Offset: {_currentOffset}, Delay: {_currentReactionDelay}, Speed: {_currentAISpeed})");
    }

    void moveByAI()
    {
        if (ball == null) return;

        if (Time.time < lastMoveTime + _currentReactionDelay)
        {
            return;
        }

        if (ball.position.z > transform.position.z + _currentOffset)
        {
            rb.linearVelocity = Vector3.forward * _currentAISpeed;
            lastMoveTime = Time.time;
        }
        else if (ball.position.z < transform.position.z - _currentOffset)
        {
            rb.linearVelocity = Vector3.forward * -_currentAISpeed;
            lastMoveTime = Time.time;
        }
        else
        {
            rb.linearVelocity = Vector3.forward * 0;
        }
    }

    void moveByPlayer()
    {
        bool pressedUp = Input.GetKey(this.up);
        bool pressedDown = Input.GetKey(this.down);

        bool pressedUp2= false;
        bool pressedDown2 = false;
        if (otherPlayer.isPlayerOther() == 1)
        {
           pressedUp2 = Input.GetKey(KeyCode.W);
           pressedDown2 = Input.GetKey(KeyCode.S);

        }
        if (pressedUp || pressedUp2)
        {
            rb.linearVelocity = Vector3.forward * playerSpeed;
        }
        else if (pressedDown || pressedDown2)
        {
            rb.linearVelocity = Vector3.forward * -playerSpeed;
        }
        else
        {
            rb.linearVelocity = Vector3.forward * 0;
        }
    }
}