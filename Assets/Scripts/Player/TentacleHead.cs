using UnityEngine;

public class TentacleHead : MonoBehaviour
{
    private Tentacle owner;

    public void SetOwner(Tentacle tentacle)
    {
        owner = tentacle;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        owner.HandleHeadCollision(collision);
    }
}
