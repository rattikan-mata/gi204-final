using UnityEngine;

public class ChildTrigger : MonoBehaviour
{
    private QuestionManager questionManager;
    private bool triggered = false;

    void Start()
    {
        questionManager = FindFirstObjectByType<QuestionManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        questionManager.ShowQuestion(gameObject);
    }
}