using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    public float tapForce = 10f;  // แรงผลักต่อการกด F หนึ่งครั้ง
    public float jumpForce = 20f; // แรงกระโดด
    public bool canMove = true;
    bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 1f; // ตัวแปร m (มวล)
    }

    void Update()
    {
        // ถ้ารถถูกสั่งให้หยุด (เช่น ตอนเจอเด็ก) ให้ความเร็วแนวนอนเป็น 0
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 1. กด F เพื่อพุ่งไปข้างหน้า (กดรัวๆ = ออกแรงซ้ำๆ รถยิ่งเร็ว)
        if (Input.GetKeyDown(KeyCode.F))
        {
            // ฟิสิกส์ F = ma (มวล x แรงผลักต่อครั้ง)
            float force = rb.mass * tapForce;
            rb.AddForce(Vector2.right * force, ForceMode2D.Impulse);
        }

        // 2. กระโดด (Spacebar)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * (rb.mass * jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        isGrounded = true;

        // ระบบความหนืด (Drag) จะส่งผลกับการกด F รัวๆ ของผู้เล่นโดยตรง
        if (col.gameObject.CompareTag("Lava"))
            rb.linearDamping = 5f; // หนืดมาก: รถจะหยุดเร็ว ต้องกด F รัวมากๆ

        else if (col.gameObject.CompareTag("Ice"))
            rb.linearDamping = 0.5f; // ลื่น: รถจะไม่ค่อยหยุด กด F ทีเดียวไหลยาวๆ

        else
            rb.linearDamping = 2f; // ถนนปกติ: ปล่อยปุ่ม F สักพักรถจะหยุดเอง
    }
}