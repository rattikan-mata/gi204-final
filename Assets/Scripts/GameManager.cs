using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;
    public int kidsAnswered = 0;

    public TextMeshProUGUI scoreText; // ลาก Text คะแนนมาใส่
    public GameObject endPanel;       // ลาก Panel สรุปคะแนนมาใส่

    void Awake() { instance = this; }

    public void AddScore(bool correct)
    {
        if (correct) score++;
        kidsAnswered++;
        scoreText.text = "คะแนน: " + score;

        if (kidsAnswered >= 5)
        { // ตอบครบ 5 คน โชว์หน้าสรุป
            endPanel.SetActive(true);
        }
    }

    public void NextScene() // ลากไปใส่ปุ่ม "ด่านต่อไป"
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}