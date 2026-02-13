using UnityEngine;

public class TentacleHead : MonoBehaviour, IMoveReceiver
{
    private Tentacle owner;
    [SerializeField] private Transform spawnFxPoint;

    public void SetOwner(Tentacle tentacle)
    {
        owner = tentacle;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionInfo colInfo = new CollisionInfo  (collision, spawnFxPoint, transform.up);
        owner.HandleHeadCollision(colInfo);
    }

    public void AddMovementSource(IMoveGiver moveGiver)
    {
        owner.RegisterMoveGiver(moveGiver);
    }

    public void RemoveMovementSource(IMoveGiver moveGiver)
    {
        owner.UnregisterMoveGiver(moveGiver);
    }

    public void DisableCollider()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}

public struct CollisionInfo
{
    public Collision2D collision2D;
    public Transform spawnPointFX;
    public Vector2 headDir;

    public CollisionInfo(Collision2D collision2D, Transform spawnPointFX, Vector2 headDir)
    {
        this.collision2D = collision2D;
        this.spawnPointFX = spawnPointFX;
        this.headDir = headDir;
    }
}
