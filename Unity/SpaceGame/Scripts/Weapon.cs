//This script is used to move the weapon and destroy it if it goes out of the screen or hits an object with the tag of "Enemy"

using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float speed = 7f; 
    public float lifetime = 3f; 
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime); 

        // Destroy if it goes past the screen
        Vector3 screenPosition = cam.WorldToViewportPoint(transform.position);
        if (screenPosition.y > 1.1f || screenPosition.y < -0.1f) 
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
