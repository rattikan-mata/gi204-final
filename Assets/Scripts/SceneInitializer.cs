using UnityEngine;

/// <summary>
/// SceneInitializer — ติดใน Scene แต่ละซีน
/// รีเซ็ตค่า childrenAnswered และ score ให้ถูกต้อง
/// เรียกเมื่อ Scene โหลด
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    [Header("Scene Info")]
    public int sceneNumber = 1;  // ซีนที่ 1 หรือ 2

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentScene = sceneNumber;
            GameManager.Instance.score = 0;
            GameManager.Instance.childrenAnswered = 0;
        }

        // คืนเวลา (กันกรณี timeScale = 0 ค้างจากซีนก่อน)
        Time.timeScale = 1f;

        Debug.Log($"[SceneInitializer] Scene {sceneNumber} initialized.");
    }
}