using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RacketController : MonoBehaviour
{   
    public float speed; // Speed of the racket movement

    public KeyCode up;
    public KeyCode down;

    public bool isPlayer = true;
    public float offset = 0.2f;
    private Rigidbody rb;
    private Transform ball;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ball = GameObject.FindGameObjectWithTag("Ball").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayer)
        {
            moveByPlayer();
        }
        else
        {
            moveByAI();
        }

    }

    void moveByAI()
    {
        if(ball.position.z > transform.position.z + offset)
        {
            rb.linearVelocity = Vector3.forward * speed;
        }
        else if(ball.position.z < transform.position.z - offset)
        {
            rb.linearVelocity = Vector3.forward * -speed;
        }
        else
        {
            rb.linearVelocity = Vector3.forward * 0;
        }
    }
    void moveByPlayer() { 
        bool pressedUp = Input.GetKey(this.up);
        bool pressedDown = Input.GetKey(this.down);
        
        if (pressedUp)
        {   
            rb.linearVelocity = Vector3.forward * speed;
        }
        else if (pressedDown)
        {
            rb.linearVelocity = Vector3.forward * -speed;
        }
        else
        {
            rb.linearVelocity = Vector3.forward * 0;
        }
    }
}
