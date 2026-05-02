using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public GameObject questionPanel;
    public TMP_Text questionText;

    public Button[] answerButtons = new Button[4];
    public TMP_Text[] answerButtonTexts = new TMP_Text[4];

    public GameObject feedbackPanel;
    public TMP_Text feedbackText;

    public GameObject loadingPanel;
    public string questionTopic = "ฟิสิกส์ระดับมัธยมต้น เรื่องแรงและการเคลื่อนที่";

    private CarController carController;
    private ScoreManager scoreManager;
    private AIQuestionService aiService;

    private GameObject currentChild;
    private int questionCount = 0;
    private int currentCorrectIndex = 0;

    void Start()
    {
        carController = FindFirstObjectByType<CarController>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
        aiService = AIQuestionService.Instance;

        if (carController == null) Debug.LogError("[QM] ไม่พบ CarController!");
        if (scoreManager == null) Debug.LogError("[QM] ไม่พบ ScoreManager!");
        if (aiService == null) Debug.LogError("[QM] ไม่พบ AIQuestionService! ใส่ใน Scene ด้วย");

        SetPanelActive(questionPanel, false);
        SetPanelActive(feedbackPanel, false);
        SetPanelActive(loadingPanel, false);
    }

    public void ShowQuestion(GameObject child)
    {
        currentChild = child;
        carController?.SetCanMove(false);

        SetPanelActive(loadingPanel, true);

        if (aiService != null)
        {
            aiService.GetQuestion(questionTopic, OnQuestionReceived);
        }
        else
        {
            OnQuestionReceived(GetFallbackQuestion(), null);
        }
    }

    private void OnQuestionReceived(AIQuestionService.QuestionData q, string error)
    {
        SetPanelActive(loadingPanel, false);

        if (error != null || q == null)
        {
            Debug.LogWarning("[QM] AI ล้มเหลว ใช้ fallback: " + error);
            q = GetFallbackQuestion();
        }

        DisplayQuestion(q);
    }

    private void DisplayQuestion(AIQuestionService.QuestionData q)
    {
        currentCorrectIndex = q.correctIndex;

        if (questionText != null)
            questionText.text = q.question;
        else
            Debug.LogError("[QM] questionText ยังไม่ผูกใน Inspector!");

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) { Debug.LogError($"[QM] answerButtons[{i}] null"); continue; }

            if (i < q.choices.Length)
            {
                answerButtons[i].gameObject.SetActive(true);

                if (answerButtonTexts[i] != null)
                    answerButtonTexts[i].text = q.choices[i];

                int captured = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(captured));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        SetPanelActive(questionPanel, true);
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        SetPanelActive(questionPanel, false);

        bool correct = (selectedIndex == currentCorrectIndex);

        if (correct)
        {
            scoreManager?.AddScore();
            ShowFeedback("CORRECT! Ice cream for you! 🍦");
        }
        else
        {
            ShowFeedback("WRONG! Maybe next time!");
        }

        questionCount++;
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null) feedbackText.text = message;
        SetPanelActive(feedbackPanel, true);
        StartCoroutine(HideFeedbackAfterDelay(1.5f));
    }

    private IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetPanelActive(feedbackPanel, false);

        if (currentChild != null) Destroy(currentChild);

        scoreManager?.CheckAllAnswered(questionCount);
        carController?.SetCanMove(true);
    }

    private static readonly string[][] fallbackPool = {
    new[] { "What is the approximate gravitational acceleration of the Earth?",
            "5 m/s²", "9.8 m/s²", "15 m/s²", "20 m/s²" },

    new[] { "What does the formula F = ma represent?",
            "Force = Mass ÷ Acceleration", "Force = Mass × Acceleration",
            "Force = Mass + Acceleration", "Force = Mass − Acceleration" },

    new[] { "How does the Ice surface affect the car?",
            "Makes it faster", "Makes it slower", "Makes it slippery and hard to control", "Stops the car immediately" },

    new[] { "What happens when the car drives on Lava?",
            "The car jumps higher", "The car glides smoothly", "The car slows down and stops quickly", "The car flips over" },

    new[] { "What is Newton's First Law of Motion?",
            "F = ma", "An object will remain at rest or move at a constant velocity unless acted upon by a force",
            "For every action, there is an equal and opposite reaction", "Energy cannot be destroyed" },
};
    private int fallbackIndex = 0;

    private AIQuestionService.QuestionData GetFallbackQuestion()
    {
        var row = fallbackPool[fallbackIndex % fallbackPool.Length];
        fallbackIndex++;

        return new AIQuestionService.QuestionData
        {
            question = row[0],
            choices = new[] { row[1], row[2], row[3], row[4] },
            correctIndex = 1
        };
    }

    private static void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
}