using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private HealthSystem hs;

    private void Awake()
    {
        hs = GetComponent<HealthSystem>();
    }

    public void TakeDamage(int damage)
    {
        hs.TakeDamage(damage);
    }
}
