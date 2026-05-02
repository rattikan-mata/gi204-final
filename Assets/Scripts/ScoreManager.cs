using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreDisplayText;
    public GameObject scoreResultPanel;
    public TMP_Text scoreResultText;

    public Button nextSceneButton;
    public int totalChildren = 3;

    private int score = 0;
    private int answeredCount = 0;

    void Start()
    {
        score = 0;
        answeredCount = 0;

        if (scoreResultPanel != null)
            scoreResultPanel.SetActive(false);
        else
            Debug.LogError("[ScoreManager] scoreResultPanel ยังไม่ผูกใน Inspector!");

        if (nextSceneButton != null)
            nextSceneButton.onClick.AddListener(LoadNextScene);
        else
            Debug.LogError("[ScoreManager] nextSceneButton ยังไม่ผูกใน Inspector!");

        UpdateScoreDisplay();
    }

    public void AddScore()
    {
        score++;
        UpdateScoreDisplay();
    }

    public void CheckAllAnswered(int answered)
    {
        answeredCount = answered;
        if (answeredCount >= totalChildren)
            StartCoroutine(ShowResultAfterDelay(0.5f));
    }

    private void UpdateScoreDisplay()
    {
        if (scoreDisplayText != null)
            scoreDisplayText.text = "🍦 " + score + " / " + totalChildren;
    }

    private IEnumerator ShowResultAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowResult();
    }

    private void ShowResult()
    {
        if (scoreResultPanel == null || scoreResultText == null) return;

        scoreResultPanel.SetActive(true);

        string stars = "";
        for (int i = 0; i < score; i++) stars += "⭐";
        for (int i = score; i < totalChildren; i++) stars += "🌑";

        scoreResultText.text =
            "คะแนน : " + score + " / " + totalChildren + "\n" + stars;
    }

    private void LoadNextScene()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(0);
    }
}