using UnityEngine;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    public static QuestionUI instance;
    public GameObject panel;
    public TextMeshProUGUI qText;
    public TextMeshProUGUI[] btnTexts; // ลาก Text ของปุ่มทั้ง 4 มาใส่

    Child currentChild;

    void Awake() { instance = this; panel.SetActive(false); }

    public void ShowQuestion(Child child)
    {
        currentChild = child;
        qText.text = child.question;
        for (int i = 0; i < 4; i++)
        {
            btnTexts[i].text = child.choices[i];
        }
        panel.SetActive(true);
    }

    // เอาฟังก์ชันนี้ไปใส่ใน OnClick() ของปุ่มทั้ง 4 (แล้วใส่เลข 0, 1, 2, 3 ตามลำดับ)
    public void Answer(int index)
    {
        bool correct = (index == currentChild.correctAns);
        GameManager.instance.AddScore(correct);
        FindObjectOfType<Truck>().canMove = true; // ให้รถวิ่งต่อ
        panel.SetActive(false); // ปิดหน้าต่างคำถาม
    }
}