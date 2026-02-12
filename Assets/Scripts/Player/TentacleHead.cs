using UnityEngine;

public class TentacleHead : MonoBehaviour, IMoveReceiver
{
    private Tentacle owner;
    [SerializeField] private ParticleSystem hitFx;
    [SerializeField] private Transform spawnFxPoint;

    public void SetOwner(Tentacle tentacle)
    {
        owner = tentacle;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        owner.HandleHeadCollision(collision);

        if(collision.gameObject.CompareTag("Wall"))
        {
            TriggerHitVisuals(spawnFxPoint.position, spawnFxPoint.rotation);
        }
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

    //todo this manages only visuals and shouldnt care about the tentacle type
    public void TriggerHitVisuals(Vector3 pos, Quaternion rot)
    {
        ParticleSystem fxInstance = Instantiate(hitFx, pos, rot).GetComponent<ParticleSystem>();
        fxInstance.Play();
        Destroy(fxInstance.gameObject, fxInstance.main.duration);
    }
}
