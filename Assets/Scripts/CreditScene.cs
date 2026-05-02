using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditScene : MonoBehaviour
{
    public Button playAgainButton;

    void Start()
    {
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);
    }

    private void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}