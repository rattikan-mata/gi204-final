using UnityEngine;

/// <summary>
/// TruckController — ควบคุมรถไอติมด้วยแรง Physics (Keyboard Only)
/// ฟิสิกส์ที่ใช้:
///   A) F = ma  → คำนวณแรงขับเคลื่อนก่อน AddForce()
///   B) OnCollisionEnter2D / OnCollisionStay2D → ตรวจพื้นผิว (Lava/Ice/Normal)
///   C) Projectile jump → ใช้ AddForce Impulse แนวตั้ง
///   D) Surface drag → เปลี่ยน linearDrag ตามพื้นผิว
///
/// Controls:
///   RightArrow / D  → เร่งความเร็ว
///   LeftArrow  / A  → เบรก / ถอยหลัง
///   Space / Up      → กระโดด
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class TruckController : MonoBehaviour
{
    // ─── Drive Parameters ───────────────────────────────────────────────
    [Header("Drive")]
    [Tooltip("มวลรถ (kg) — ใช้ใน F = ma")]
    public float mass = 1200f;

    [Tooltip("ความเร่งที่ต้องการ (m/s²)")]
    public float targetAcceleration = 8f;

    [Tooltip("ความเร่งถอยหลัง/เบรก (m/s²)")]
    public float brakeAcceleration = 5f;

    [Tooltip("ความเร็วสูงสุด (m/s)")]
    public float maxSpeed = 12f;

    // ─── Jump Parameters ─────────────────────────────────────────────────
    [Header("Jump")]
    [Tooltip("ความเร็วกระโดด (m/s)")]
    public float jumpSpeed = 10f;

    [Tooltip("LayerMask ของพื้น")]
    public LayerMask groundLayer;

    [Tooltip("จุดตรวจว่าอยู่บนพื้น (Empty child ใต้รถ)")]
    public Transform groundCheck;

    [Tooltip("รัศมีตรวจพื้น")]
    public float groundCheckRadius = 0.2f;

    // ─── Surface Modifiers ───────────────────────────────────────────────
    [Header("Surface")]
    [Tooltip("ค่า drag ปกติ")]
    public float normalDrag = 1f;

    [Tooltip("ค่า drag บนลาวา (หนืด)")]
    public float lavaDrag = 8f;

    [Tooltip("ค่า drag บนน้ำแข็ง (ลื่น)")]
    public float iceDrag = 0.05f;

    // ─── Wheel Visual ────────────────────────────────────────────────────
    [Header("Wheels (optional)")]
    public Transform wheelFront;
    public Transform wheelBack;
    public float wheelRotateSpeed = 200f;

    // ─── Internal ────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private bool isGrounded;
    private SurfaceType currentSurface = SurfaceType.Normal;
    private bool inputEnabled = true;

    public enum SurfaceType { Normal, Lava, Ice }

    // ─────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.linearDamping = normalDrag;
    }

    private void Update()
    {
        CheckGrounded();
        RotateWheels();
    }

    private void FixedUpdate()
    {
        if (!inputEnabled) return;
        HandleDrive();
        HandleJump();
    }

    // ─── Ground Check ────────────────────────────────────────────────────
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ─── Drive: F = m × a ────────────────────────────────────────────────
    private void HandleDrive()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            horizontal = 1f;
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            horizontal = -1f;

        if (horizontal == 0f) return;

        // จำกัดความเร็วสูงสุด
        if (Mathf.Abs(rb.linearVelocity.x) >= maxSpeed &&
            Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(horizontal))
            return;

        float accel = (horizontal > 0) ? targetAcceleration : brakeAcceleration;
        float surfaceMultiplier = GetSurfaceAccelMultiplier();

        // ── F = m × a (Newton's 2nd Law) ──
        float force = mass * accel * surfaceMultiplier * horizontal;
        rb.AddForce(new Vector2(force, 0f), ForceMode2D.Force);
    }

    // ─── Jump: Impulse แนวตั้ง ───────────────────────────────────────────
    private void HandleJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            // F = m × v (Impulse = mass × jumpSpeed)
            float jumpImpulse = mass * jumpSpeed;
            rb.AddForce(new Vector2(0f, jumpImpulse), ForceMode2D.Impulse);
        }
    }

    // ─── Surface ─────────────────────────────────────────────────────────
    private float GetSurfaceAccelMultiplier()
    {
        return currentSurface switch
        {
            SurfaceType.Lava => 0.3f,
            SurfaceType.Ice => 1.5f,
            _ => 1.0f
        };
    }

    private void OnCollisionEnter2D(Collision2D col) => ApplySurface(col.gameObject.tag);
    private void OnCollisionStay2D(Collision2D col) => ApplySurface(col.gameObject.tag);
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Lava") || col.gameObject.CompareTag("Ice"))
            SetSurface(SurfaceType.Normal);
    }

    private void ApplySurface(string tag)
    {
        switch (tag)
        {
            case "Lava": SetSurface(SurfaceType.Lava); break;
            case "Ice": SetSurface(SurfaceType.Ice); break;
            default: SetSurface(SurfaceType.Normal); break;
        }
    }

    private void SetSurface(SurfaceType surface)
    {
        if (currentSurface == surface) return;
        currentSurface = surface;

        switch (surface)
        {
            case SurfaceType.Lava:
                rb.linearDamping = lavaDrag;
                rb.angularDamping = lavaDrag;
                break;
            case SurfaceType.Ice:
                rb.linearDamping = iceDrag;
                rb.angularDamping = 0.1f;
                break;
            default:
                rb.linearDamping = normalDrag;
                rb.angularDamping = 0.5f;
                break;
        }

        UIManager.Instance?.ShowSurfaceHint(surface.ToString());
        Debug.Log($"[Truck] Surface → {surface}, drag={rb.linearDamping}");
    }

    // ─── Wheel Visual ────────────────────────────────────────────────────
    private void RotateWheels()
    {
        float rot = -rb.linearVelocity.x * wheelRotateSpeed * Time.deltaTime;
        if (wheelFront) wheelFront.Rotate(0f, 0f, rot);
        if (wheelBack) wheelBack.Rotate(0f, 0f, rot);
    }

    // ─── Enable / Disable Input ──────────────────────────────────────────
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        if (!enabled)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}