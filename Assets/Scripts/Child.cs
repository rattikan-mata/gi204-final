using UnityEngine;

public class Child : MonoBehaviour
{
    public string question = "1 + 1 = ?";
    public string[] choices = { "1", "2", "3", "4" };
    public int correctAns = 1; // ข้อถูก (เริ่มนับที่ 0) เช่น 1 คือตอบ "2"

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Truck"))
        {
            col.GetComponent<Truck>().canMove = false; // หยุดรถ
            QuestionUI.instance.ShowQuestion(this);    // โชว์คำถาม
            gameObject.SetActive(false);               // ปิดตัวเด็ก
        }
    }
}