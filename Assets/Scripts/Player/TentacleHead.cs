using UnityEngine;

public class TentacleHead : MonoBehaviour, IMoveReceiver
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

    public void AddMovementSource(IMoveGiver moveGiver)
    {
        owner.RegisterMoveGiver(moveGiver);
    }

    public void RemoveMovementSource(IMoveGiver moveGiver)
    {
        owner.UnregisterMoveGiver(moveGiver);
    }
}
