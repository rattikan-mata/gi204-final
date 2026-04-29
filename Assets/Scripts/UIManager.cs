using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UIManager — จัดการ UI ทั้งหมดในซีนเกม (Keyboard version)
/// - HUD: Score, Surface Hint
/// - ScorePanel: แสดงคะแนนท้ายซีน
/// ไม่มีปุ่ม Mobile
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // ─── HUD ─────────────────────────────────────────────────────────────
    [Header("HUD")]
    public TMP_Text scoreText;           // "🍦 0 / 5"
    public TMP_Text surfaceHintText;     // "⚠ พื้นลาวา!"
    public float hintDuration = 2f;

    // ─── Score Panel ─────────────────────────────────────────────────────
    [Header("Score Panel")]
    public GameObject scorePanel;
    public TMP_Text scorePanelText;      // "คะแนนของคุณ: 3 / 5"
    public TMP_Text scoreStarsText;      // "⭐⭐⭐☆☆"
    public Button nextSceneButton;

    // ─── Controls Hint ───────────────────────────────────────────────────
    [Header("Controls Hint (optional)")]
    public TMP_Text controlsHintText;    // แสดง "← → เร่ง/เบรก | Space กระโดด"

    private Coroutine hintCoroutine;

    // ─────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        scorePanel?.SetActive(false);
        surfaceHintText?.gameObject.SetActive(false);
        UpdateScoreHUD();

        nextSceneButton?.onClick.AddListener(
            () => GameManager.Instance?.GoToNextScene());

        if (controlsHintText != null)
            controlsHintText.text = "← → เร่ง / เบรก    |    Space กระโดด";
    }

    // ─── Score HUD ────────────────────────────────────────────────────────
    public void UpdateScoreHUD()
    {
        if (GameManager.Instance == null || scoreText == null) return;
        int s = GameManager.Instance.score;
        int t = GameManager.Instance.totalChildren;
        scoreText.text = $"🍦 {s} / {t}";
    }

    // ─── Surface Hint ─────────────────────────────────────────────────────
    public void ShowSurfaceHint(string surface)
    {
        if (surfaceHintText == null) return;

        string msg = surface switch
        {
            "Lava" => "🔥 พื้นลาวา! รถหนืดลง",
            "Ice" => "🧊 พื้นน้ำแข็ง! รถลื่น",
            _ => ""
        };

        if (string.IsNullOrEmpty(msg))
        {
            surfaceHintText.gameObject.SetActive(false);
            return;
        }

        if (hintCoroutine != null) StopCoroutine(hintCoroutine);
        surfaceHintText.text = msg;
        surfaceHintText.gameObject.SetActive(true);
        hintCoroutine = StartCoroutine(HideHintAfter(hintDuration));
    }

    private System.Collections.IEnumerator HideHintAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        surfaceHintText?.gameObject.SetActive(false);
    }

    // ─── Score Panel ──────────────────────────────────────────────────────
    public void ShowScorePanel(int score, int total)
    {
        if (scorePanel == null) return;

        FindFirstObjectByType<TruckController>()?.SetInputEnabled(false);

        scorePanelText.text = $"คะแนนของคุณ\n{score}  /  {total}";

        string stars = "";
        for (int i = 0; i < total; i++)
            stars += (i < score) ? "⭐" : "☆";
        scoreStarsText.text = stars;

        if (nextSceneButton != null)
        {
            var label = nextSceneButton.GetComponentInChildren<TMP_Text>();
            bool isLast = GameManager.Instance?.currentScene >= 2;
            if (label) label.text = isLast ? "ดูเครดิต ▶" : "ซีนถัดไป ▶";
        }

        scorePanel.SetActive(true);
        Time.timeScale = 0f;
    }
}