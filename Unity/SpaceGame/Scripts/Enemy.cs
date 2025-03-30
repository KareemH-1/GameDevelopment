//Add a Collider2D and set IsTrigger = true and Add a Rigidbody2D Gravity Scale = 0 on enemy and set to kinematic
//add rigid body and box collider to player and weapon as well and set to kinematic , and on bullet set isTrigger on
//Tag the Player as "Player".
//Tag the Bullet as "Weapon".
//Add a Collider2D and set IsTrigger = true on enemy
//Add a Rigidbody2D Gravity Scale = 0 on enemy
//add rigid body and box collider to player and weapon as well
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
            ScoreManager.instance.AddScore(10); // Add 10 score points
            Destroy(other.gameObject); // Destroy the bullet

            PowerUpSpawner spawner = FindAnyObjectByType<PowerUpSpawner>();
            if (spawner != null)
            {
                spawner.TrySpawnPowerUp(transform.position); // Spawn power-up at enemy position
            }

            Destroy(gameObject); // Destroy the enemy
        }
        else if (other.CompareTag("Player"))
        {
            GameOverManager.instance.GameOver(ScoreManager.instance.GetScore()); // Trigger game over
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
