using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerHealthSystem : HealthSystem
{
    [SerializeField] private List<HealthDisplay> healthDisplay;
    [SerializeField] private PlayerHitVisual hitVisual;
    [SerializeField] private float invulnerabilityDuration = 2f;
    private bool isInvulnerable = false;

    public override void TakeDamage(int damage)
    {
        if(isInvulnerable)
        {
            RaiseOnHit();
            return;
        }

        base.TakeDamage(damage);
        TriggerInvulnerability();
    }

    private void TriggerInvulnerability()
    {
        StartCoroutine(InvulnerabilityCoroutine());
        hitVisual.Activate(invulnerabilityDuration);
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    protected override void UpdateHealthVisuals(int health)
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
