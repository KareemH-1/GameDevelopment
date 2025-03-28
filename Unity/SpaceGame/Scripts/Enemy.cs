//Add to enemy prefab

//Add a Collider2D and set IsTrigger = true and Add a Rigidbody2D Gravity Scale = 0 on enemy and set to kinematic
//add rigid body and box collider to player and weapon as well and set to kinematic , and on bullet set isTrigger on
//Tag the Player as "Player".
//Tag the Bullet as "Weapon".
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public AudioClip deathSound;
    private AudioSource audioSource;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = deathSound;
    }

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        Vector3 screenPosition = cam.WorldToViewportPoint(transform.position);
        if (screenPosition.y < -0.1f) 
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon")) 
        {
            ScoreManager.instance.AddScore(10); // add 10 score points
            Destroy(other.gameObject); // destroy rocket
            Destroy(gameObject); // Destroy enemy
        }
        else if (other.CompareTag("Player"))
        {

            GameOverManager.instance.GameOver(ScoreManager.instance.GetScore()); // Call GameOver
            Destroy(other.gameObject); // Destroy Player
            Destroy(gameObject); // Destroy Enemy
        }
    }
    void PlayDeathSound()
    {
        audioSource.Play(); 
        Destroy(gameObject, deathSound.length);
    }
}
