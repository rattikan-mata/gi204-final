using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        public string question;
        public string[] choices;      // ตัวเลือก 4 ข้อ
        public int correctIndex;      // index ของคำตอบถูก (0-3)
    }

    public List<QuestionData> questions = new List<QuestionData>();

    public GameObject questionPanel;
    public Text questionText;
    public Button[] answerButtons;        // ปุ่มตัวเลือก 4 ปุ่ม
    public Text[] answerButtonTexts;      // Text บนแต่ละปุ่ม
    public GameObject feedbackPanel;
    public Text feedbackText;

    private CarController carController;
    private ScoreManager scoreManager;
    private GameObject currentChild;
    private int currentQuestionIndex = 0;
    private int questionCount = 0;

    void Start()
    {
        carController = FindFirstObjectByType<CarController>();
        scoreManager = FindFirstObjectByType<ScoreManager>();

        questionPanel.SetActive(false);
        feedbackPanel.SetActive(false);

        if (questions.Count == 0)
            LoadDefaultQuestions();
    }

    public void ShowQuestion(GameObject child)
    {
        currentChild = child;
        carController.SetCanMove(false);

        int idx = Random.Range(0, questions.Count);
        currentQuestionIndex = idx;
        QuestionData q = questions[idx];

        questionText.text = q.question;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < q.choices.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtonTexts[i].text = q.choices[i];

                int capturedIndex = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(capturedIndex));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        questionPanel.SetActive(true);
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        questionPanel.SetActive(false);

        bool correct = (selectedIndex == questions[currentQuestionIndex].correctIndex);

        if (correct)
        {
            scoreManager.AddScore();
            ShowFeedback("? Correct! Got Free Ice Cream ??");
        }
        else
        {
            ShowFeedback("? Wrong! Don't get Ice Cream ??");
        }

        questionCount++;
    }

    private void ShowFeedback(string message)
    {
        feedbackPanel.SetActive(true);
        feedbackText.text = message;
        StartCoroutine(HideFeedbackAfterDelay(1.5f));
    }

    private IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackPanel.SetActive(false);

        if (currentChild != null)
            Destroy(currentChild);

        scoreManager.CheckAllAnswered(questionCount);

        carController.SetCanMove(true);
    }

    private void LoadDefaultQuestions()
    {
        questions.Add(new QuestionData
        {
            question = "แรงโน้มถ่วงโลกมีค่าประมาณเท่าไร?",
            choices = new[] { "5 m/s?", "9.8 m/s?", "15 m/s?", "20 m/s?" },
            correctIndex = 1
        });
        questions.Add(new QuestionData
        {
            question = "F = ma หมายถึงอะไร?",
            choices = new[] { "แรง = มวล ? ความเร็ว", "แรง = มวล ? ความเร่ง",
                              "แรง = มวล + ความเร่ง", "แรง = มวล ? ความเร่ง" },
            correctIndex = 1
        });
        questions.Add(new QuestionData
        {
            question = "วัตถุที่มีมวลมากกว่าจะมีแรงเสียดทานมากกว่าหรือไม่?",
            choices = new[] { "ใช่เสมอ", "ไม่ใช่เสมอ — ขึ้นกับสัมประสิทธิ์แรงเสียดทาน",
                              "ไม่ — มวลไม่เกี่ยว", "ขึ้นกับความเร็ว" },
            correctIndex = 1
        });
        questions.Add(new QuestionData
        {
            question = "พื้นน้ำแข็งทำให้รถลื่นเพราะอะไร?",
            choices = new[] { "มวลลดลง", "แรงโน้มถ่วงเพิ่มขึ้น",
                              "แรงเสียดทานต่ำมาก", "ความเร็วสูงขึ้น" },
            correctIndex = 2
        });
        questions.Add(new QuestionData
        {
            question = "กฎข้อ 1 ของนิวตันกล่าวว่า?",
            choices = new[] { "F = ma", "วัตถุจะอยู่นิ่งหรือเคลื่อนที่สม่ำเสมอถ้าไม่มีแรงสุทธิ",
                              "แรงกิริยาเท่ากับแรงปฏิกิริยา", "พลังงานสร้างหรือทำลายไม่ได้" },
            correctIndex = 1
        });
    }
}