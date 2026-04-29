using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager - ควบคุมสถานะหลักของเกม (Score, Scene Flow)
/// เป็น Singleton ที่ persist ข้ามซีน
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentScene = 1;         // ซีนที่ 1 หรือ 2
    public int totalChildren = 5;        // จำนวนเด็กต่อซีน
    public int childrenAnswered = 0;     // เด็กที่ตอบถูกแล้ว
    public int score = 0;               // คะแนนในซีนปัจจุบัน

    [Header("Scene Names")]
    public string scene1Name = "Scene1";
    public string scene2Name = "Scene2";
    public string creditSceneName = "Credits";

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// เรียกเมื่อเด็กตอบคำถามถูก
    /// </summary>
    public void ChildAnsweredCorrectly()
    {
        score++;
        childrenAnswered++;
        Debug.Log($"[GameManager] Score: {score}/{totalChildren}");

        if (childrenAnswered >= totalChildren)
        {
            // จบซีน — แสดง ScorePanel ก่อนไปซีนถัดไป
            UIManager.Instance?.ShowScorePanel(score, totalChildren);
        }
    }

    /// <summary>
    /// เรียกเมื่อเด็กตอบผิด (ไม่เพิ่มคะแนน)
    /// </summary>
    public void ChildAnsweredWrong()
    {
        childrenAnswered++;
        Debug.Log($"[GameManager] Wrong! Score: {score}/{totalChildren}");

        if (childrenAnswered >= totalChildren)
        {
            UIManager.Instance?.ShowScorePanel(score, totalChildren);
        }
    }

    /// <summary>
    /// โหลดซีนถัดไปหลังดูคะแนนแล้ว
    /// </summary>
    public void GoToNextScene()
    {
        score = 0;
        childrenAnswered = 0;

        if (currentScene == 1)
        {
            currentScene = 2;
            SceneManager.LoadScene(scene2Name);
        }
        else
        {
            // จบเกม ไปซีน Credits
            SceneManager.LoadScene(creditSceneName);
        }
    }

    /// <summary>
    /// รีสตาร์ทเกมใหม่ตั้งแต่ต้น
    /// </summary>
    public void RestartGame()
    {
        currentScene = 1;
        score = 0;
        childrenAnswered = 0;
        SceneManager.LoadScene(scene1Name);
    }
}