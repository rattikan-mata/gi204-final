using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ลากรถ (Truck) มาใส่ช่องนี้
    public Vector3 offset = new Vector3(3f, 2f, -10f); // ระยะห่างของกล้อง

    void LateUpdate()
    {
        // ถ้ามีรถให้ตาม ก็เซ็ตตำแหน่งกล้องเท่ากับตำแหน่งรถ + ระยะห่าง
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}