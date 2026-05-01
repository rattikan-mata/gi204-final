using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Transform player;
    public float fallThreshold = -100f;
    public Vector3 respawnPoint;

    private Rigidbody2D rb;

    void Start()
    {
        if (player != null)
        {
            rb = player.GetComponent<Rigidbody2D>();
            respawnPoint = player.position;
        }
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.y < fallThreshold)
        {
            Respawn();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        player.position = respawnPoint;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        player.rotation = Quaternion.identity;

        Debug.Log("Respawned!");
    }

    public void SetNewCheckpoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }
}