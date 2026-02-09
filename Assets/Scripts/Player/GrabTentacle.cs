using UnityEngine;

public class GrabTentacle : Tentacle
{
    [SerializeField] private IGrabbable grabbedObject;

    private void Start()
    {
        OnTentacleDestroyed += ReleaseGrabbedObject;
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

    public void ReleaseGrabbedObject()
    {
        Vector3 releaseDir = -tentacleHead.up;
        float releaseSpeed = isRetracting ? retractSpeed : 0f;

        grabbedObject?.OnGrabEnd(releaseSpeed * releaseDir);
        grabbedObject = null;
    }

    protected override void ForceRetractTentacle()
    {
        base.ForceRetractTentacle();
        ReleaseGrabbedObject();
    }

    public override void HandleHeadCollision(Collision2D collision)
    {
        if(collision.transform.CompareTag("Wall"))
        {
            grabbedObject?.OnGrabEnd(Vector3.zero);
            canExpand = false;
            forceRetract = true;
        }
        
        if(grabbedObject == null && !forceRetract)
        {
            IGrabbable grabbed = collision.transform.GetComponent<IGrabbable>();
            if(grabbed != null)
            {
                grabbedObject = grabbed;
                grabbedObject.OnGrabStart(tentacleHead);
                canExpand = false;
            }
        }
    }
}
