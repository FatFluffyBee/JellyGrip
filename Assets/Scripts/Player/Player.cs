using UnityEngine;

public class Player : MonoBehaviour, IPushable, IDamageable
{
    private HealthSystem hs;
    private Movement movement;
    private Knockback knockback;

    private void Awake()
    {
        hs = GetComponent<HealthSystem>();
        movement = GetComponent<Movement>();
        knockback = GetComponent<Knockback>();

        movement.AddMovementSource(knockback);
    }

    public void Push(Vector3 pushForce)
    {
        knockback.Apply(pushForce);
    }

    public void TakeDamage(int damage)
    {
        hs.TakeDamage(damage);
    }
}
