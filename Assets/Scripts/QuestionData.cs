using UnityEngine;

/// <summary>
/// QuestionData — ScriptableObject เก็บข้อมูลคำถามหนึ่งข้อ
/// สร้างได้จาก: Assets > Create > IceCreamGame > Question
/// แล้วลากไปใส่ ChildTrigger ใน Inspector ได้เลย
/// </summary>
[CreateAssetMenu(
    fileName = "NewQuestion",
    menuName = "IceCreamGame/Question",
    order = 0)]
public class QuestionData : ScriptableObject
{
    [Header("Question")]
    [TextArea(2, 5)]
    public string questionText = "คำถาม?";

    [Header("Choices (ใส่อย่างน้อย 2 ข้อ, สูงสุด 4 ข้อ)")]
    public string choiceA = "A";
    public string choiceB = "B";
    public string choiceC = "C";
    public string choiceD = "D";

    [Header("Correct Answer")]
    [Tooltip("0=A  1=B  2=C  3=D")]
    [Range(0, 3)]
    public int correctIndex = 0;

    /// <summary>
    /// แปลงเป็น string[] สำหรับ QuestionManager
    /// </summary>
    public string[] GetChoicesArray()
        => new[] { choiceA, choiceB, choiceC, choiceD };
}