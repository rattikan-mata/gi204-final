using UnityEngine;

/// <summary>
/// ChildTriggerWithData — เวอร์ชันอัปเกรดของ ChildTrigger
/// รองรับ QuestionData ScriptableObject
/// ใช้แทน ChildTrigger ได้เลย (เลือกอันใดอันหนึ่ง)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ChildTriggerWithData : MonoBehaviour
{
    [Header("Question (ScriptableObject)")]
    public QuestionData questionData;

    [Header("Sprites")]
    public Sprite happySprite;
    public Sprite sadSprite;

    [Header("Ice Cream Reward (optional)")]
    public GameObject iceCreamPrefab;      // spawn ไอติม เมื่อตอบถูก

    private bool triggered = false;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    // ─── Physics Coding: OnTriggerEnter2D ────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Truck")) return;
        if (questionData == null)
        {
            Debug.LogWarning($"[ChildTriggerWithData] {name}: ไม่มี QuestionData!");
            return;
        }

        triggered = true;

        // หยุดรถ
        other.GetComponent<TruckController>()?.SetInputEnabled(false);

        // เปิดคำถาม
        QuestionManager.Instance?.ShowQuestion(
            questionData.questionText,
            questionData.GetChoicesArray(),
            questionData.correctIndex,
            null  // ไม่ใช้ ChildTrigger parameter
        );

        // Register callback ด้วยตัวเอง
        StartCoroutine(WaitForAnswer(questionData.correctIndex));
    }

    private System.Collections.IEnumerator WaitForAnswer(int correctIdx)
    {
        // รอให้ QuestionManager ปิด panel
        while (QuestionManager.Instance != null &&
               QuestionManager.Instance.questionPanel.activeSelf)
        {
            yield return null;
        }

        // อ่านคำตอบจาก QuestionManager ผ่าน event (ใช้ event pattern)
        // ในที่นี้ใช้ static flag อย่างง่าย
        bool correct = QuestionManager.LastAnswerCorrect;
        OnAnswered(correct);
    }

    public void OnAnswered(bool correct)
    {
        if (sr != null)
            sr.sprite = correct ? happySprite : sadSprite;

        if (correct)
        {
            GameManager.Instance?.ChildAnsweredCorrectly();
            UIManager.Instance?.UpdateScoreHUD();

            // Spawn ไอติม
            if (iceCreamPrefab != null)
                Instantiate(iceCreamPrefab,
                            transform.position + Vector3.up * 1.5f,
                            Quaternion.identity);
        }
        else
        {
            GameManager.Instance?.ChildAnsweredWrong();
        }
    }
}