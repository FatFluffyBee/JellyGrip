using System.Collections.Generic;
using UnityEngine;

public class MoveTentacle : Tentacle
{
    [Header("Auto Retract")]
    [SerializeField] private bool retractWhenOnTarget;
    [SerializeField] private float targetReachDistance = 0.5f;

    [Header("Pull Force")]
    [SerializeField] private float initialPullStrenght;
    [SerializeField] private float maxPullStrenght;
    [SerializeField] private float minPullStrength;
    
    [SerializeField] private float maxRangeWhenGrabbing = 7;

    private bool isHooked = false;

    private IMoveReceiver hookedObjectMoveReceiver;
    private HookPull playerToHook;
    private HookPull objectToPlayer;

    protected override void ApplyChildVisuals() //! first thing to refactor
    {
        if(isHooked)
        {
            if(retractWhenOnTarget)
            {
                if(Vector2.Distance(root.position, tentacleHead.position) < targetReachDistance)
                {
                    ForceRetract();
                    return;
                }
            }
            
            float targetSegmentSize = Vector2.Distance(root.position, tentacleHead.position) / (basePoses.Count - 1) * 1f;
            currentSegmentSize = Mathf.Lerp(currentSegmentSize, targetSegmentSize, 100f * Time.deltaTime);
        }
        else
        {
            currentSegmentSize = Vector2.Distance(root.position, tentacleHead.position) / (basePoses.Count - 1) * 1f;
        }

        ApplyFABRIK(tentacleHead.position, basePoses, 5, currentSegmentSize);
    }

    public override void TryExpand()
    {     
        if(!isHooked && canExpand)
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
        isHooked = false;
        stopMovement = false;
        tentacleHead.transform.SetParent(null);

        if(playerToHook != null)
        {
            ownerMoveReceiver.RemoveMovementSource(playerToHook);
        }

        if(objectToPlayer != null)
        {
            hookedObjectMoveReceiver.RemoveMovementSource(objectToPlayer);
        }
    }

    public override void HandleHeadCollision(CollisionInfo colInfo)
    {
        HookAnchor hookAnchor = colInfo.collision2D.collider.GetComponent<HookAnchor>();

        if(!isHooked)
        {
            if(hookAnchor != null && !forceRetract)
            {
                //more of an assurance to not grab for one frame and trigger sound and fx
                if(hookAnchor.IsDanger && !canTouchDangerouseSurface)
                    return;

                FirstHitVisuals(colInfo);
                StartHookState();
                hookAnchor.AttachTentacle(this);
                tentacleHead.transform.SetParent(hookAnchor.FollowableParent);

                HookPullConfig config = new HookPullConfig
                {
                    initialPullForce = initialPullStrenght,
                    maxConstantPullForce = maxPullStrenght,
                    minConstantPullForce = minPullStrength,
                    maxRangePull = maxRangeWhenGrabbing,
                    minRangePull = targetReachDistance
                };

                playerToHook = new HookPull(root, tentacleHead, config);
                ownerMoveReceiver.AddMovementSource(playerToHook);

                hookedObjectMoveReceiver = hookAnchor.GetComponent<IMoveReceiver>();
                if(hookedObjectMoveReceiver != null)
                {
                    objectToPlayer = new HookPull(hookAnchor.transform, root, config);
                    hookAnchor.GetComponent<IMoveReceiver>().AddMovementSource(objectToPlayer);
                }
            }
        }
    }

    private void StartHookState()
    {
        isHooked = true;
        applyForces = false;
        forceExpand = false;
        canExpand = false;
        wiggleAmplitude /= 2f;
        maxTentacleRange = maxRangeWhenGrabbing;
        stopMovement = true;
    }

    public void FirstHitVisuals(CollisionInfo colInfo)
    {
        AudioManager.Instance.PlayOneShot(tentacleHitWallAudio);
        ParticleSystem fxInstance = Instantiate(
            wallHitFX, 
            colInfo.spawnPointFX.position, 
            colInfo.spawnPointFX.rotation)
            .GetComponent<ParticleSystem>();
        fxInstance.Play();
        Destroy(fxInstance.gameObject, fxInstance.main.duration);

        ShakeObject shakeObject = new ShakeObject(shakeDuration, shakeIntensity, shakeFrequency, colInfo.headDir);
        shakeEvent.Raise(shakeObject);
    }
}
