using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// CreditsManager — ควบคุมซีน Credits
/// ข้อความเลื่อนขึ้น (scroll) แล้วมีปุ่มเล่นใหม่
/// </summary>
public class CreditsManager : MonoBehaviour
{
    [Header("Credits Content")]
    public RectTransform creditsContent;   // RectTransform ของ Text container
    public float scrollSpeed = 50f;        // px ต่อวินาที

    [Header("UI")]
    public Button playAgainButton;
    public TMP_Text finalScoreText;        // แสดงคะแนนรวม (optional)

    [Header("Scene")]
    public string mainMenuScene = "Scene1";

    private bool scrolling = true;

    private void Start()
    {
        Time.timeScale = 1f;

        // แสดงคะแนนรวม (ถ้ามี)
        if (finalScoreText != null && GameManager.Instance != null)
        {
            // GameManager อาจเก็บคะแนนรวมได้ — ในที่นี้แสดงข้อความขอบคุณ
            finalScoreText.text = "🍦 ขอบคุณที่เล่นเกม IceCream Truck! 🍦";
        }

        playAgainButton?.onClick.AddListener(PlayAgain);
    }

    private void Update()
    {
        if (!scrolling || creditsContent == null) return;

        // เลื่อนขึ้น
        creditsContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // หยุดเมื่อสุดท้าย (ประมาณ)
        if (creditsContent.anchoredPosition.y >= creditsContent.rect.height * 0.5f)
        {
            scrolling = false;
        }
    }

    private void PlayAgain()
    {
        GameManager.Instance?.RestartGame();
    }
}