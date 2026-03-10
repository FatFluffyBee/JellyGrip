using System.Collections.Generic;
using UnityEngine;

public class MoveTentacle : Tentacle, IMoveGiver
{
    [Header("Auto Retract")]
    [SerializeField] private bool retractWhenOnTarget;
    [SerializeField] private float targetReachDistance = 0.5f;

    [Header("Pull Force")]
    [SerializeField] private float initialPullStrenght;
    [SerializeField] private float maxPullStrenght;
    [SerializeField] private float minPullStrength;
    
    [SerializeField] private float maxRangeWhenGrabbing = 7;

    private bool isGrabbing = false;
    private bool firstImpact = true;

    public override void InitializeTentacle(Transform root, Vector3 targetDir)
    {
        base.InitializeTentacle(root, targetDir);
        forceExpand = true;
    }

    protected override void ApplyChildVisuals() //! first thing to refactor
    {
        if(isGrabbing)
        {
            if(retractWhenOnTarget)
            {
                if(Vector2.Distance(root.position, tentacleHead.position) < targetReachDistance)
                {
                    DestroyTentacle();
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
        stopMovement = false;
        tentacleHead.transform.SetParent(null);
    }

    public override List<MoveInput> CalculateMovementToGive(MoveReceiverData moveReceiverData)
    {
        Vector3 moveReceiverPos = moveReceiverData.centerPoint;
        List<MoveInput> moveInputs = new List<MoveInput>();
        if(isGrabbing)
        {
            Vector2 rootToHead = tentacleHead.position - moveReceiverPos;
            Vector2 pullDir = rootToHead.normalized;

            if(firstImpact)
            {
                moveInputs.Add(new MoveInput(pullDir * initialPullStrenght, MoveType.Impulse));
                firstImpact = false;
            }

            float pullForce = CalculatePullForce(rootToHead.magnitude);
            moveInputs.Add(new MoveInput(pullDir * pullForce, MoveType.Acceleration));
        }

        moveInputs.Add(new MoveInput(Vector3.zero, MoveType.Acceleration));
        return moveInputs;
    }

    private float CalculatePullForce(float tentacleLength)
    {
        float force = Mathf.InverseLerp(targetReachDistance, maxRangeWhenGrabbing, tentacleLength);
        force = 1 - (1 - force) * (1 - force) * (1 - force);
        force = Mathf.Lerp(minPullStrength, maxPullStrenght, force);
        return force;
    }

    public override void HandleHeadCollision(CollisionInfo colInfo)
    {
        HookAnchor hookAnchor = colInfo.collision2D.collider.GetComponent<HookAnchor>();

        if(!isGrabbing)
        {
            if(hookAnchor != null && !forceRetract)
            {
                //more of an assurance to not grab for one frame and trigger sound and fx
                if(hookAnchor.IsDanger && !canTouchDangerouseSurface)
                    return;

                FirstHitVisuals(colInfo);
                StartGrabState();
                hookAnchor.AttachTentacle(this);
                tentacleHead.transform.SetParent(hookAnchor.FollowableParent);

                if(hookAnchor.GetComponent<Movement>())
                {
                    hookAnchor.GetComponent<Movement>().AddMovementSource(this);
                }
            }
        }
    }

    private void StartGrabState()
    {
        isGrabbing = true;
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
