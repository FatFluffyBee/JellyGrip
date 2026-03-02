using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    public event Action OnDeath;
    public event Action OnHit;

    private void Start()
    {
        currentHealth = maxHealth;   
        UpdateHealthVisuals(currentHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateHealthVisuals(0);
            OnDeath?.Invoke();
            return;
        }

        OnHit?.Invoke();
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
}
