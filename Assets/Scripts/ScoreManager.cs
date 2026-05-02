using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public Text scoreDisplayText;
    public GameObject scoreResultPanel;
    public Text scoreResultText;
    public Button nextSceneButton;

    public int totalChildren = 5;

    private int score = 0;
    private int answeredCount = 0;

    void Start()
    {
        score = 0;
        answeredCount = 0;
        scoreResultPanel.SetActive(false);
        UpdateScoreDisplay();

        nextSceneButton.onClick.AddListener(LoadNextScene);
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
        {
            StartCoroutine(ShowResultAfterDelay(0.5f));
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreDisplayText != null)
            scoreDisplayText.text = "?? " + score + " / " + totalChildren;
    }

    private IEnumerator ShowResultAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowResult();
    }

    private void ShowResult()
    {
        scoreResultPanel.SetActive(true);

        string stars = "";
        for (int i = 0; i < score; i++) stars += "?";

        scoreResultText.text = "Score: " + score + " / " + totalChildren + "\n" + stars;
    }

    private void LoadNextScene()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(0);
    }
}