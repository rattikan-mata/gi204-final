using UnityEngine;

/// <summary>
/// CameraFollow — กล้องตามรถแบบ Smooth Lerp
/// ติดกับ Main Camera ในซีนเกม
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;  // ลาก TruckController ใส่ใน Inspector

    [Header("Offset")]
    public Vector3 offset = new Vector3(3f, 2f, -10f);

    [Header("Smooth")]
    [Range(1f, 20f)]
    public float smoothSpeed = 5f;

    [Header("Bounds (ป้องกันกล้องออกนอกแผนที่)")]
    public bool useBounds = false;
    public float minX = -10f;
    public float maxX = 200f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;

        if (useBounds)
            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);

        // ล็อค Y บางส่วนเพื่อไม่ให้กล้องกระเด้งมาก
        desiredPos.y = Mathf.Lerp(transform.position.y, desiredPos.y,
                                  smoothSpeed * 0.5f * Time.deltaTime);
        desiredPos.x = Mathf.Lerp(transform.position.x, desiredPos.x,
                                  smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(desiredPos.x, desiredPos.y, offset.z);
    }
}