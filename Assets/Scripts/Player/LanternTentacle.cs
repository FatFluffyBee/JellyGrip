using UnityEngine;

public class LanternTentacle : Tentacle
{
    [Header("Light Detection")]
    [SerializeField] private float activationRange;
    [SerializeField] private float checkIntervals;

    private void Start()
    {
        InvokeRepeating(nameof(CheckForLight), 0f, 0.2f);
    }

    public override void TryExpand()
    {
        if(canExpand)
        {
            isExpanding = true;
        }
    }

    public override void TryRetract()
    {
        isRetracting = true;
    }

    public void CheckForLight()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(tentacleHead.position, activationRange);
        if(hits.Length > 0)
        {
            foreach(Collider2D hit in hits)
            {
                 if(hit.CompareTag("Light"))
                {
                    hit.GetComponent<LightBulb>().Activate();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(tentacleHead.position, activationRange);
    }

    public override void HandleHeadCollision(Collision2D collision)
    {
        if(collision.transform.CompareTag("Wall"))
        {
            canExpand = false;
            ForceRetract();
        }
    }
}
