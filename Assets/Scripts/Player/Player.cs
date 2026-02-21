using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    private HealthSystem hs;
    
    private Movement movement;
    [SerializeField] private MonoBehaviour tentacleManager; //! for testing only
    private Knockback knockback;

    [SerializeField] private float knockbackForce = 10f;
    
    //public event Action OnObstacleHit;

    private void Awake()
    {
        hs = GetComponent<HealthSystem>();
        movement = GetComponent<Movement>();
        knockback = GetComponent<Knockback>();

        movement.AddMovementSource(tentacleManager as IMoveGiver);
        movement.RemoveMovementSource(knockback);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        /*if(col.collider.CompareTag("Ennemi"))
        {
            tentacleManager.RetractAllTentacles();
            hs.TakeDamage(1);
            knockback.Apply((Vector3)col.GetContact(0).point, knockbackForce);
        }*/

        /*if(col.collider.CompareTag("Wall"))
        {
            tentacleManager.RetractAllTentacles();
        }*/
    }
}
