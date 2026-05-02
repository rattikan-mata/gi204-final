using UnityEngine;

public class SurfaceEffect : MonoBehaviour
{
    public enum SurfaceType { Normal, Lava, Ice }
    public SurfaceType surfaceType = SurfaceType.Normal;

    private const float DRAG_NORMAL = 1f;
    private const float DRAG_LAVA = 10f;
    private const float DRAG_ICE = 0.05f;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        ApplyDrag(rb);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float targetDrag = GetTargetDrag();
        if (!Mathf.Approximately(rb.linearDamping, targetDrag))
            ApplyDrag(rb);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        rb.linearDamping = DRAG_NORMAL;
    }

    private float GetTargetDrag()
    {
        return surfaceType switch
        {
            SurfaceType.Lava => DRAG_LAVA,
            SurfaceType.Ice => DRAG_ICE,
            _ => DRAG_NORMAL
        };
    }

    private void ApplyDrag(Rigidbody2D rb)
    {
        rb.linearDamping = GetTargetDrag();
        Debug.Log($"[SurfaceEffect] {surfaceType}: drag = {rb.linearDamping}");
    }
}