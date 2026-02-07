using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private List<HealthDisplay> healthDisplay;
    [SerializeField] private int maxHealth;
    private int currentHealth;
    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;   
        Updatehealth(currentHealth);
        OnDeath += () => Debug.Log("Dead");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Updatehealth(0);
            OnDeath?.Invoke();
            return;
        }

        Updatehealth(currentHealth);
    }

    public void Heal(int healthGain)
    {
        currentHealth += healthGain;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Updatehealth(currentHealth);
    }

    public void Updatehealth(int health)
    {
        for(int i = 0; i < health; i++)
        {
            healthDisplay[i].Activate();
        }

        for(int i = health; i < healthDisplay.Count; i++)
        {
            healthDisplay[i].Deactivate();
        }
    }
}
