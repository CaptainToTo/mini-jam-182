using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float dist;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + (-Vector3.forward * dist);
        // Vector2 forward = new Vector2(player.transform.forward.x, player.transform.forward.z).normalized;
        // transform.up = new Vector3(forward.x, 0, forward.y);
    }
}
