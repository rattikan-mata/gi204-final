using UnityEngine;

public class CarController : MonoBehaviour
{
    public float accelerationPerPress = 5f;
    public float jumpForce = 17.5f;
    public float maxSpeed = 25f;

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
            float mass = rb.mass;
            float forceMagnitude = mass * accelerationPerPress;
            rb.AddForce(Vector2.right * forceMagnitude, ForceMode2D.Impulse);
        }

        if (rb.linearVelocity.x > maxSpeed)
        {
            rb.linearVelocity = new Vector2(maxSpeed, rb.linearVelocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * (rb.mass * jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!value)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        isGrounded = true;
    }
}