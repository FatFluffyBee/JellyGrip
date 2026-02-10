using UnityEngine;

public class MoveTentacle : Tentacle
{
    [SerializeField] private float grabSpeed;

    private bool isGrabbing = false;

    protected override void ApplyChildPhysics()
    {
        if(isGrabbing)
        {
            float targetSegmentSize = Vector3.Distance(root.position, tentacleHead.position) / ((basePoses.Count - 1) * 1f);
            currentSegmentSize = Mathf.Lerp(currentSegmentSize, targetSegmentSize, 100f * Time.deltaTime);

            if(Vector2.Distance(root.position, tentacleHead.position) < 0.5f)
            {
                DestroyTentacle();
                return;
            }

            ApplyFABRIK(tentacleHead.position, basePoses, 1, currentSegmentSize);
        }
    }

    public override void TryExpand()
    {     
        if(!isGrabbing && canExpand)
        {
            forceExpand = true;
        }
    }

    public override void TryRetract()
    {
        ForceRetract();
    }

    public override void ForceRetract()
    {
        base.ForceRetract();
        isGrabbing = false;
    }

    public override Vector3 GetDesiredMovement()
    {
        if(isGrabbing)
        {
            Vector3 dirToRoot = tentacleHead.position - root.position;
            dirToRoot.ToV2Dir();
            return dirToRoot * grabSpeed;
        }

        return Vector3.zero;
    }

    public override void HandleHeadCollision(Collision2D collision)
    {
        if(collision.transform.CompareTag("Wall"))
        {
            isGrabbing = true;
            applyForces = false;
            forceExpand = false;
            canExpand = false;
        }
    }
}
