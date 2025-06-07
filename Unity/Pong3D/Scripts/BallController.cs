using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed = 8f;
    public float maxSpeed = 30f;

    private float currentSpeed;
    private Vector3 direction;
    public float minDirection = 0.5f;
    public bool stopped = true;
    public GameObject SparkVFX;
    private Rigidbody rb;
    public AudioClip hitSoundClip;
    private AudioSource audioSource;

    public float bounceDampening = 0.9f;
    public float wallZMin = -7f;
    public float wallZMax = 7f;

    public float maxRacketBounceAngleFactor = 0.7f;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();

        if (!rb.isKinematic)
        {
            rb.isKinematic = true;
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        currentSpeed = initialSpeed;
    }

    void FixedUpdate()
    {
        if (stopped)
        {
            return;
        }

        rb.MovePosition(rb.position + direction * currentSpeed * Time.fixedDeltaTime);

        if (rb.position.z < wallZMin)
        {
            direction.z = Mathf.Abs(direction.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, wallZMin);
            ApplyBounceEffect();
        }
        else if (rb.position.z > wallZMax)
        {
            direction.z = -Mathf.Abs(direction.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, wallZMax);
            ApplyBounceEffect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Racket"))
        {
            Collider racketCollider = other.GetComponent<Collider>();
            if (racketCollider == null)
            {
                return;
            }

            float hitPointZ = transform.position.z - other.transform.position.z;
            float racketHalfLength = racketCollider.bounds.extents.z;
            float normalizedHitZ = hitPointZ / racketHalfLength;

            Vector3 newDirection = Vector3.zero;

            newDirection.x = -direction.x;
            newDirection.z = normalizedHitZ * maxRacketBounceAngleFactor;

            newDirection.x = Mathf.Sign(newDirection.x) * Mathf.Max(Mathf.Abs(newDirection.x), this.minDirection);

            if (Mathf.Abs(newDirection.z) < this.minDirection / 2f && normalizedHitZ == 0)
            {
                newDirection.z = (Random.value > 0.5f ? 1f : -1f) * (this.minDirection / 2f);
            }

            direction = newDirection.normalized;

            ApplyBounceEffect();
            IncreaseSpeed(0.2f);
        }
    }

    private void ApplyBounceEffect()
    {
        GameObject spark = Instantiate(SparkVFX, transform.position, transform.rotation);
        Destroy(spark, 4f);
        PlayHitSound();
    }

    private void PlayHitSound()
    {
        if (hitSoundClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSoundClip);
        }
    }

    private void ChooseDirection()
    {
        float signX = Random.value > 0.5f ? 1f : -1f;
        float signZ = Random.value > 0.5f ? 1f : -1f;

        this.direction = new Vector3(
            signX * Random.Range(minDirection, 1f),
            0,
            signZ * Random.Range(minDirection, 1f)
        ).normalized;
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
        ChooseDirection();
        stopped = false;

    }
    public void IncreaseSpeed(float speedIncreasePerPoint)
    {
        currentSpeed = Mathf.Min(currentSpeed + speedIncreasePerPoint, maxSpeed);
        Debug.Log($"Ball speed increased to: {currentSpeed}");
    }

    public void ResetSpeed()
    {
        currentSpeed = initialSpeed;
        Debug.Log($"Ball speed reset to initial: {currentSpeed}");
    }
}