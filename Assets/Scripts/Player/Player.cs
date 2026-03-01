using UnityEngine;

public class Player : MonoBehaviour
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

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Ennemi"))
        {
            hs.TakeDamage(1);
            knockback.Apply((Vector3)col.GetContact(0).point, knockbackForce);
        }
    }
}
