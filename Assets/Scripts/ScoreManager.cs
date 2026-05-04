using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreResultText;
    public GameObject scoreResultPanel;
    public Button nextSceneButton;

    private int score = 0;
    private int maxScoreDisplay = 3;

    void Start()
    {
        scoreResultPanel.SetActive(false);
        nextSceneButton.onClick.AddListener(() => SceneManager.LoadScene("Credit"));
    }

    public void AddScore() => score++;

    public void CheckAllAnswered(int count)
    {
        if (count >= 3)
        {
            ShowResult();
        }
    }

    private void ShowResult()
    {
        scoreResultPanel.SetActive(true);
        scoreResultText.text = $"Total Score\n{score} / {maxScoreDisplay}";
    }
}