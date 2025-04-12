using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public Butterfly player;
    public float hitThreshold;

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Wall") && col.impulse.magnitude > hitThreshold)
        {
            player.HitWall(col.GetContact(0).point, col.GetContact(0).normal);
        }
    }
}
