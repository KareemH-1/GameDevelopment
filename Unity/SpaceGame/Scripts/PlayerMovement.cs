//This scripts allows the ship to move around the screen with restrictions.
//There is also a nitro feature that allows the ship to move faster.
using UnityEngine;
public class playerMovement : MonoBehaviour
{
    public float rotationAngle = 10f;  // Rotation step in degrees
    public float moveSpeed = 5f;       // Forward/backward movement speed
    public float nitroSpeed = 9f;      // Nitro movement speed
    public float horizontalSpeed = 3f; // Left/right movement speed
    public float horizontalNitroSpeed = 5f; // Nitro left/right movement speed
    public float rotationSpeed = 5f;   // Smooth rotation speed
    private float targetRotation = 0f; // Target rotation angle
    private float minX, maxX, minY, maxY; // Screen boundaries

    void Start()
    {
        SetScreenBounds();
    }

    void Update()
    {
        Vector3 newPosition = transform.position;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                newPosition += Vector3.left * horizontalNitroSpeed * Time.deltaTime;
            }
            else
            {
                newPosition += Vector3.left * horizontalSpeed * Time.deltaTime;
            }
            targetRotation = rotationAngle; // Rotate left
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                newPosition += Vector3.right * horizontalNitroSpeed * Time.deltaTime;
            }
            else
            {
                newPosition += Vector3.right * horizontalSpeed * Time.deltaTime;
            }
            targetRotation = -rotationAngle; // Rotate right
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                newPosition += transform.up * nitroSpeed * Time.deltaTime;
            }
            else
            {
                newPosition += transform.up * moveSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                newPosition -= transform.up * nitroSpeed * Time.deltaTime;
            }
            else
            {
                newPosition -= transform.up * moveSpeed * Time.deltaTime;
            }
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow) &&
            !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow))
        {
            targetRotation = 0f; 
        }


        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        transform.position = newPosition;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetRotation), rotationSpeed * Time.deltaTime);
    }

    void SetScreenBounds()
    {
        Camera cam = Camera.main;
        float shipWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        float shipHeight = GetComponent<SpriteRenderer>().bounds.extents.y;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x + shipWidth;
        maxX = topRight.x - shipWidth;
        minY = bottomLeft.y + shipHeight;
        maxY = topRight.y - shipHeight;
    }
}
