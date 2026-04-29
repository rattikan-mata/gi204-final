using UnityEngine;

public class Truck : MonoBehaviour
{
    Rigidbody2D rb;
    public float acceleration = 10f;
    public float jumpForce = 8f;
    public bool canMove = true;
    bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 1f; // ตัวแปร m (มวล)
    }

    void FixedUpdate()
    {
        if (!canMove) { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); return; } // เบรก

        float move = Input.GetAxisRaw("Horizontal");
        float force = rb.mass * acceleration * move; // ทฤษฎี F = ma
        rb.AddForce(Vector2.right * force);
    }

    void Update()
    {
        if (!canMove) return;
        // กระโดด Impulse
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * (rb.mass * jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        isGrounded = true;
        // เปลี่ยนแรงเสียดทานตาม Tag พื้นผิว
        if (col.gameObject.CompareTag("Lava")) rb.linearDamping = 5f; // หนืดมาก
        else if (col.gameObject.CompareTag("Ice")) rb.linearDamping = 0.5f; // ลื่น
        else rb.linearDamping = 2f; // ถนนปกติ
    }
}