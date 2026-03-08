using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private float knockback;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IPushable pushable = collision.collider.GetComponent<IPushable>();
        if(pushable != null)
        {
            pushable.Push(transform.up * knockback);
        }

        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeDamage(1);
        } 
    }
}
