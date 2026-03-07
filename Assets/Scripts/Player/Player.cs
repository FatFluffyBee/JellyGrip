using UnityEngine;

public class Player : MonoBehaviour, IPushable, IDamageable
{
    private HealthSystem hs;
    private Movement movement;
    private Knockback knockback;

    [SerializeField] private float knockbackForce = 10f;

    private void Awake()
    {
        hs = GetComponent<HealthSystem>();
        movement = GetComponent<Movement>();
        knockback = GetComponent<Knockback>();

        movement.AddMovementSource(knockback);
    }

    public void Push(Vector3 direction, float force)
    {
        knockback.Apply(direction, force);
    }

    public void TakeDamage(int damage)
    {
        hs.TakeDamage(damage);
    }
}
