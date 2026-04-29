using UnityEngine;

/// <summary>
/// SurfaceZone — ติดกับ GameObject ที่เป็นพื้นผิวพิเศษ
/// ไม่ต้องเขียนอะไรเพิ่ม แค่ตั้ง surfaceType ใน Inspector
/// Script นี้จะตั้ง Tag ให้อัตโนมัติ และเปลี่ยนสีให้เห็นชัด
/// </summary>
public class SurfaceZone : MonoBehaviour
{
    public enum Surface { Normal, Lava, Ice }

    [Header("Surface Type")]
    public Surface surfaceType = Surface.Normal;

    [Header("Visual (ถ้ามี SpriteRenderer)")]
    public bool autoColor = true;

    private void Awake()
    {
        // ตั้ง Tag ให้อัตโนมัติ (ต้องสร้าง Tag ใน Project Settings ก่อน)
        gameObject.tag = surfaceType.ToString(); // "Normal" / "Lava" / "Ice"

        if (autoColor)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = surfaceType switch
                {
                    Surface.Lava => new Color(1f, 0.35f, 0.1f, 1f),   // ส้มแดง
                    Surface.Ice => new Color(0.6f, 0.85f, 1f, 1f),   // ฟ้าอ่อน
                    _ => Color.gray
                };
            }
        }
    }
}