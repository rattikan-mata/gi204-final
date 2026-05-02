using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float accelerationPerPress = 5f;
    public float jumpForce = 30f;
    public float maxSpeed = 20f;

    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip driveClip;
    public AudioClip jumpClip;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            float forceMagnitude = rb.mass * accelerationPerPress;
            rb.AddForce(Vector2.right * forceMagnitude, ForceMode2D.Impulse);

            if (sfxSource != null && driveClip != null)
                sfxSource.PlayOneShot(driveClip);
        }

        if (rb.linearVelocity.x > maxSpeed)
        {
            rb.linearVelocity = new Vector2(maxSpeed, rb.linearVelocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * (rb.mass * jumpForce), ForceMode2D.Impulse);
            rb.gravityScale = 3f;
            isGrounded = false;

            if (sfxSource != null && jumpClip != null)
                sfxSource.PlayOneShot(jumpClip);
        }
    }

    public void SetCanMove(bool value) => canMove = value;

    void OnCollisionEnter2D(Collision2D col)
    {
        isGrounded = true;
        rb.gravityScale = 2f;
    }
}