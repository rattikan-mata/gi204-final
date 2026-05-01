using UnityEngine;

public class SurfaceEffect : MonoBehaviour
{
    public enum SurfaceType { Normal, Lava, Ice }

    public SurfaceType surfaceType = SurfaceType.Normal;

    private const float DRAG_NORMAL = 1f;
    private const float DRAG_LAVA = 8f;
    private const float DRAG_ICE = 0.05f;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        Rigidbody2D carRb = col.gameObject.GetComponent<Rigidbody2D>();
        if (carRb == null) return;

        ApplyDrag(carRb);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        Rigidbody2D carRb = col.gameObject.GetComponent<Rigidbody2D>();
        if (carRb == null) return;

        carRb.linearDamping = DRAG_NORMAL;
    }

    private void ApplyDrag(Rigidbody2D rb)
    {
        switch (surfaceType)
        {
            case SurfaceType.Lava:
                rb.linearDamping = DRAG_LAVA;
                Debug.Log("[SurfaceEffect] ≈“«“: drag = " + DRAG_LAVA);
                break;
            case SurfaceType.Ice:
                rb.linearDamping = DRAG_ICE;
                Debug.Log("[SurfaceEffect] πÈ”·¢Áß: drag = " + DRAG_ICE);
                break;
            default:
                rb.linearDamping = DRAG_NORMAL;
                break;
        }
    }
}