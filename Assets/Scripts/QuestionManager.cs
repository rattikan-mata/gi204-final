using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// QuestionManager — ควบคุม UI ถามตอบ (อัปเดต: มี LastAnswerCorrect)
/// เป็น Singleton, ถูก call จาก ChildTrigger / ChildTriggerWithData
/// </summary>
public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    /// <summary>
    /// ผลการตอบครั้งล่าสุด (ให้ ChildTriggerWithData อ่านได้)
    /// </summary>
    public static bool LastAnswerCorrect { get; private set; }

    [Header("UI References")]
    public GameObject questionPanel;
    public TMP_Text questionText;
    public Button[] choiceButtons;
    public TMP_Text[] choiceTexts;
    public TMP_Text feedbackText;

    [Header("Feedback Colors")]
    public Color correctColor = new Color(0.2f, 0.8f, 0.2f);
    public Color wrongColor = new Color(0.9f, 0.2f, 0.2f);
    public Color normalColor = Color.white;

    [Header("Timing")]
    public float feedbackDuration = 1.5f;

    private int currentCorrectIndex;
    private ChildTrigger currentChild;
    private TruckController truck;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (questionPanel) questionPanel.SetActive(false);
        if (feedbackText) feedbackText.gameObject.SetActive(false);
    }

    public void ShowQuestion(string question, string[] choices,
                             int correctIndex, ChildTrigger child)
    {
        currentCorrectIndex = correctIndex;
        currentChild = child;
        truck = FindFirstObjectByType<TruckController>();
        LastAnswerCorrect = false;

        questionText.text = question;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int idx = i;
            bool visible = i < choices.Length;
            choiceButtons[i].gameObject.SetActive(visible);
            if (!visible) continue;

            choiceTexts[i].text = choices[i];
            choiceButtons[i].interactable = true;
            SetButtonColor(choiceButtons[i], normalColor);

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(idx));
        }

        feedbackText.gameObject.SetActive(false);
        questionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnChoiceSelected(int selectedIndex)
    {
        bool correct = (selectedIndex == currentCorrectIndex);
        LastAnswerCorrect = correct;

        SetButtonColor(choiceButtons[selectedIndex],
                       correct ? correctColor : wrongColor);
        if (!correct)
            SetButtonColor(choiceButtons[currentCorrectIndex], correctColor);

        foreach (var btn in choiceButtons)
            btn.interactable = false;

        feedbackText.gameObject.SetActive(true);
        feedbackText.text = correct
            ? "✔ ถูกต้อง! ได้รับไอติม 🍦"
            : "✘ ผิด! ไม่ได้ไอติม 😢";
        feedbackText.color = correct ? correctColor : wrongColor;

        currentChild?.OnAnswered(correct);
        UIManager.Instance?.UpdateScoreHUD();

        StartCoroutine(CloseAfterDelay(feedbackDuration));
    }

    private System.Collections.IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        questionPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        Time.timeScale = 1f;

        if (GameManager.Instance != null &&
            GameManager.Instance.childrenAnswered < GameManager.Instance.totalChildren)
        {
            truck?.SetInputEnabled(true);
        }
    }

    private void SetButtonColor(Button btn, Color color)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        cb.highlightedColor = color * 1.1f;
        btn.colors = cb;
    }
}