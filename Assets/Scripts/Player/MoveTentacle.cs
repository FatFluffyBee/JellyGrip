using System.Collections.Generic;
using UnityEngine;

public class MoveTentacle : Tentacle
{
    [Header("Auto Retract")]
    [SerializeField] private bool retractWhenOnTarget;
    [SerializeField] private float targetReachDistance = 0.5f;

    [Header("Pull Force")]
    [SerializeField] private float initialPullStrenght;
    [SerializeField] private float pullStrenghtOverTime;

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
    }

    public override List<MoveInput> GetDesiredMovement()
    {
        List<MoveInput> moveInputs = new List<MoveInput>();
        if(isGrabbing)
        {
            Vector3 dirToRoot = tentacleHead.position - root.position;
            dirToRoot.ToV2Dir();

            if(firstImpact)
            {
                moveInputs.Add(new MoveInput(dirToRoot * initialPullStrenght, MoveType.Impulse));
                firstImpact = false;
            }

            moveInputs.Add(new MoveInput(dirToRoot * pullStrenghtOverTime, MoveType.Velocity));
        }

        moveInputs.Add(new MoveInput(Vector3.zero, MoveType.Velocity));
        return moveInputs;
    }

    public override void HandleHeadCollision(CollisionInfo colInfo)
    {
        Collision2D collision = colInfo.collision2D;

        if(collision.transform.CompareTag("Wall"))
        {
            if(!isGrabbing)
            {
                OnInitialWallHitFeedback(colInfo);
                StartGrabState();
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
    }

    public void OnInitialWallHitFeedback(CollisionInfo colInfo)
    {
        AudioManager.Instance.PlayOneShot(tentacleHitWall);
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
