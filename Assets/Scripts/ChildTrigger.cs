using UnityEngine;

/// <summary>
/// ChildTrigger — ติดกับ GameObject เด็กแต่ละคน
/// เมื่อรถวิ่งมาชน Trigger จะเปิดหน้าต่างคำถาม
/// ใช้ Trigger2D (Physics Coding นับคะแนน)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ChildTrigger : MonoBehaviour
{
    [Header("Question Data")]
    [Tooltip("คำถามที่ถามเด็กคนนี้")]
    [TextArea(2, 4)]
    public string question = "2 + 2 = ?";

    [Tooltip("ตัวเลือก A–D")]
    public string[] choices = { "3", "4", "5", "6" };

    [Tooltip("index ของคำตอบที่ถูก (0=A, 1=B, 2=C, 3=D)")]
    public int correctIndex = 1;

    [Tooltip("Sprite แสดงเมื่อตอบถูก")]
    public Sprite happySprite;

    [Tooltip("Sprite แสดงเมื่อตอบผิด")]
    public Sprite sadSprite;

    private bool triggered = false;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // ตั้ง Collider เป็น Trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    // ─── OnTriggerEnter2D: Physics Coding ────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Truck")) return;

        triggered = true;

        // หยุดรถ
        TruckController truck = other.GetComponent<TruckController>();
        truck?.SetInputEnabled(false);

        // เปิดหน้าต่างคำถาม
        QuestionManager.Instance?.ShowQuestion(question, choices, correctIndex, this);
    }

    // ─── เรียกจาก QuestionManager หลังตอบ ───────────────────────────────
    public void OnAnswered(bool correct)
    {
        if (sr != null)
            sr.sprite = correct ? happySprite : sadSprite;

        // แจ้ง GameManager
        if (correct)
            GameManager.Instance?.ChildAnsweredCorrectly();
        else
            GameManager.Instance?.ChildAnsweredWrong();
    }
}