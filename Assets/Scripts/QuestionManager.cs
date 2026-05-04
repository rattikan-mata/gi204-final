using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject questionPanel;
    public TMP_Text categoryText;
    public TMP_Text questionText;
    public TMP_Text resultFeedbackText;
    public Button[] answerButtons = new Button[4];
    public TMP_Text[] answerButtonTexts = new TMP_Text[4];

    private CarController carController;
    private ScoreManager scoreManager;
    private GameObject currentChild;
    private int currentQuestionIndex = 0;

    [System.Serializable]
    public class QuestionData
    {
        public string category;
        public string question;
        public string[] choices;
        public int correctIndex;
    }

    public QuestionData[] questions = new QuestionData[] {
        new QuestionData {
            category = "Astronomy",
            question = "Which planet is known as the 'Red Planet'?",
            choices = new string[] { "Venus", "Jupiter", "Mars", "Saturn" },
            correctIndex = 2
        },
        new QuestionData {
            category = "Animals",
            question = "What is the tallest animal in the world?",
            choices = new string[] { "Elephant", "Giraffe", "Ostrich", "Kangaroo" },
            correctIndex = 1
        },
        new QuestionData {
            category = "Human Body",
            question = "Which organ is responsible for pumping blood throughout the human body?",
            choices = new string[] { "Brain", "Lungs", "Stomach", "Heart" },
            correctIndex = 3
        }
    };

    void Start()
    {
        carController = FindFirstObjectByType<CarController>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
        questionPanel.SetActive(false);
        resultFeedbackText.text = "";
    }

    public void ShowQuestion(GameObject child)
    {
        currentChild = child;
        carController?.SetCanMove(false);
        SetupUI(questions[currentQuestionIndex]);
    }

    private void SetupUI(QuestionData data)
    {
        questionPanel.SetActive(true);
        categoryText.text = "Category: " + data.category;
        questionText.text = data.question;
        resultFeedbackText.text = "";

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtonTexts[i].text = data.choices[i];
            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    private void OnAnswerSelected(int index)
    {
        int correctIdx = questions[currentQuestionIndex].correctIndex;

        if (index == correctIdx)
        {
            resultFeedbackText.text = "<color=green>CORRECT!</color>";
            scoreManager?.AddScore();
        }
        else
        {
            resultFeedbackText.text = "<color=red>WRONG!</color>";
        }

        foreach (var btn in answerButtons) btn.interactable = false;

        StartCoroutine(NextQuestionSequence());
    }

    private IEnumerator NextQuestionSequence()
    {
        yield return new WaitForSeconds(1.5f);

        questionPanel.SetActive(false);
        foreach (var btn in answerButtons) btn.interactable = true;

        if (currentChild != null) Destroy(currentChild);

        currentQuestionIndex++;
        scoreManager?.CheckAllAnswered(currentQuestionIndex);
        carController?.SetCanMove(true);
    }
}