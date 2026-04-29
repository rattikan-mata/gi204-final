using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public void RestartGame() // ลากไปใส่ปุ่ม "เล่นใหม่"
    {
        SceneManager.LoadScene(0); // กลับไปโหลด Scene ด่าน 1 (ที่ index 0)
    }
}