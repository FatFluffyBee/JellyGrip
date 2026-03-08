using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    public event Action OnDeath;
    public event Action OnHit;
    public event Action OnDamage;

    private void Start()
    {
        currentHealth = maxHealth;   
        UpdateHealthVisuals(currentHealth);
        OnHit += () => Debug.Log("Hit taken, current health: " + currentHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        OnHit?.Invoke();
        OnDamage?.Invoke();

        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateHealthVisuals(0);
            OnDeath?.Invoke();
            return;
        }

        
        UpdateHealthVisuals(currentHealth);
    }

    public void Heal(int healthGain)
    {
        currentHealth += healthGain;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthVisuals(currentHealth);
    }

    protected virtual void UpdateHealthVisuals(int health)
    {
        
    }

    protected void RaiseOnHit() //! might not be needed with delegate, investigate when time
    {
        OnHit?.Invoke();
    }
}
