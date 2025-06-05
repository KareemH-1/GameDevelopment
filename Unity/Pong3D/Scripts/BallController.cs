using UnityEngine;

public class BallController : MonoBehaviour
{
    // --- NEW SPEED VARIABLES ---
    public float initialSpeed = 8f; 
    public float speedIncreasePerPoint = 1f; // How much speed increases after each point
    public float maxSpeed = 30f; // The maximum speed the ball can reach

    private float currentSpeed;


    private Vector3 direction;
    public float minDirection = 0.5f;
    public bool stopped = true;
    public GameObject SparkVFX;
    private Rigidbody rb;
    public AudioClip hitSoundClip;
    private AudioSource audioSource;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        // Initialize currentSpeed to the initial speed when the component starts
        currentSpeed = initialSpeed;
    }

    void FixedUpdate()
    {
        if (stopped)
        {
            return;
        }
        rb.MovePosition(rb.position + direction * currentSpeed * Time.fixedDeltaTime); // Use currentSpeed
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hit = false;
        if (other.CompareTag("Racket"))
        {
            Vector3 newDirection = (transform.position - other.transform.position).normalized;
            newDirection.x = Mathf.Sign(newDirection.x) * Mathf.Max(Mathf.Abs(newDirection.x), this.minDirection);
            newDirection.z = Mathf.Sign(newDirection.z) * Mathf.Max(Mathf.Abs(newDirection.z), this.minDirection);
            hit = true;
            direction = newDirection;
        }

        if (other.CompareTag("Wall"))
        {
            hit = true;
            direction.z = -direction.z;
        }

        if (hit)
        {
            GameObject spark = Instantiate(SparkVFX, transform.position, transform.rotation);
            Destroy(spark, 4f);

            if (hitSoundClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSoundClip);
            }
        }
    }

    private void ChooseDirection()
    {
        float signX = Mathf.Sign(Random.Range(-1f, 1f));
        float signZ = Mathf.Sign(Random.Range(-1f, 1f));

        this.direction = new Vector3(signX * 0.5f, 0, signZ * 0.5f);
    }

    public void stop()
    {
        stopped = true;
    }

    public void EnableMovement()
    {
        stopped = false;
    }
    public void StartNewPoint()
    {
        ChooseDirection(); // Choose a new random direction
        stopped = false;    // Enable movement
    }

    public void IncreaseSpeed()
    {
        currentSpeed = Mathf.Min(currentSpeed + speedIncreasePerPoint, maxSpeed);
        Debug.Log($"Ball speed increased to: {currentSpeed}"); // Optional: for debugging
    }

    public void ResetSpeed()
    {
        currentSpeed = initialSpeed;
        Debug.Log($"Ball speed reset to initial: {currentSpeed}");
    }
}