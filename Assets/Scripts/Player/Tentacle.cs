using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tentacle : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] protected float shootSpeed;
    [SerializeField] protected float minDistTentaclesPoints;
    [SerializeField] protected GameObject tentacleHeadPrefab;
    [SerializeField] protected GameObject tentacleBodyPrefab;
    [SerializeField] protected float maxAnglePerSecond;

    [Header("Retract")]
    [SerializeField] protected float retractSpeed;
    [SerializeField] protected float lengthBeforeDelete = 0.5f;

    [Header("Range")]
    [SerializeField] protected float maxTentacleRange = 5f;
    [SerializeField] protected bool autoRetractOnRangeReach;

    [Header("Wiggle")]
    [SerializeField] protected float wiggleSpeed;
    [SerializeField] protected float wiggleAmplitude;
    [SerializeField] protected float wiggleFrequency;
    [SerializeField] protected float endDampening;
    [SerializeField] protected float smoothFactor;

    [Header("Audio")]
    [SerializeField] protected AudioAssetSO fireTentacle;
    [SerializeField] protected AudioAssetSO retractTentacle;
    [SerializeField] protected AudioAssetSO tentacleHitWall;

    [Header("FX")]
    [SerializeField] protected GameObject wallHitFX;

    [Header("ScreenShake")]
    [SerializeField] protected ScreenshakeEventSO shakeEvent;
    [SerializeField] protected float shakeDuration;
    [SerializeField] protected float shakeIntensity;
    [SerializeField] protected float shakeFrequency;


    protected bool canExpand = true;
    protected bool isExpanding = false;
    protected bool forceExpand = false;
    protected bool isRetracting = false;
    protected bool forceRetract = false;
    protected bool applyForces = true;
    protected bool hasReachMaxRange = false;

    protected Vector3 shootDir;
    protected Vector3 targetDir;
    protected float currentSegmentSize; 
    private float segmentBeforeDelete;
    private float tentacleLength;
    public bool IsDead {get; private set;}

    protected List<Vector3> basePoses = new List<Vector3>();
    protected List<Vector3> targetPoses = new List<Vector3>(); //wiggled ones
    protected List<Vector3> currentPoses = new List<Vector3>();
    protected List<GameObject> segments = new List<GameObject>();


    protected Rigidbody2D tentacleHeadRb;
    protected Transform tentacleHead;
    private TentacleHead tentacleHeadHandler;
    protected Transform root;

    private List<IMoveGiver> moveGivers = new List<IMoveGiver>();
    protected List<MoveInput> moveInputs = new List<MoveInput>();
    

    public event Action OnTentacleDestroyed;
    public event Action<Tentacle> OnForceRetract;

    public virtual void TryExpand(){}

    public virtual void TryRetract() {}

    public void RegisterMoveGiver(IMoveGiver moveGiver)
    {
        if(!moveGivers.Contains(moveGiver))
        {
            moveGivers.Add(moveGiver);
        }
    }

    public void UnregisterMoveGiver(IMoveGiver moveGiver)
    {
        if(moveGivers.Contains(moveGiver))
        {
            moveGivers.Remove(moveGiver);
        }
    }

    private void FixedUpdate()
    {
        Vector3 newHeadPos = tentacleHead.position;

        //Add idle drift for head
        if(applyForces)
        {
            Vector3 velocityTotal = Vector3.zero;
            Vector3 impulseTotal = Vector3.zero;
            foreach(IMoveGiver list in moveGivers)
            {
                foreach(MoveInput e in list.GetDesiredMovement())
                {
                    if(e.moveType == MoveType.Velocity)
                        velocityTotal += e.input;
                    else if(e.moveType == MoveType.Impulse)
                        impulseTotal += e.input;
                }
            }

            newHeadPos += velocityTotal * Time.deltaTime + impulseTotal;

            float t = velocityTotal.magnitude / 10f;
            float maxStep = maxAnglePerSecond * Time.deltaTime * t;
            float angle = Vector2.SignedAngle(shootDir, velocityTotal.normalized);
            shootDir = Quaternion.Euler(0, 0, Mathf.Clamp(angle, -maxStep, maxStep)) * shootDir;
            shootDir.Normalize();
        }

        if(isRetracting || forceRetract)
        {
            if(RetractAlongPath(newHeadPos, out Vector3 finalHeadPos))
            {
                DestroyTentacle();
                return;
            }
            newHeadPos = finalHeadPos;
            isRetracting = false;

            Vector3 newShootDir = basePoses[^1] - basePoses[^2];
            if(newShootDir.magnitude > 0.0001f)
            {
                shootDir = newShootDir.normalized;
            }
        }
        else if((isExpanding || forceExpand) && !hasReachMaxRange)
        {
            AdvanceShootDirection(targetDir);
            newHeadPos += ComputeExpansionDelta();
            isExpanding = false;
        }

        tentacleHeadRb.MovePosition(newHeadPos);
        tentacleHead.up = shootDir;
    }

    private void LateUpdate()
    {   
        UpdateStartTentaclePoses();

        UpdateEndTentaclePoses();

        ApplyChildVisuals();

        UpdateWigglePosesAndTentacleLength();
        UpdateCurrentPoses();
        UpdateSegments();
        UpdateRangeStatus();
    }

    protected virtual void ApplyChildVisuals()
    {
        
    }

    public virtual void InitializeTentacle(Transform root, Vector3 targetDir)
    {
        this.targetDir = targetDir;
        shootDir = targetDir;

        tentacleHead = Instantiate(tentacleHeadPrefab, transform.position, Quaternion.identity).transform;
        tentacleHeadRb = tentacleHead.GetComponent<Rigidbody2D>();
        this.root = root;
        
        tentacleHeadHandler = tentacleHead.GetComponent<TentacleHead>();
        tentacleHeadHandler.SetOwner(this);

        basePoses.Add(transform.position);
        currentPoses.Add(transform.position);
        targetPoses.Add(transform.position);

        basePoses.Add(transform.position);
        currentPoses.Add(transform.position);
        targetPoses.Add(transform.position);
        
        currentSegmentSize = minDistTentaclesPoints;
        segmentBeforeDelete = lengthBeforeDelete / minDistTentaclesPoints;

        AddNewSegment(transform.position, transform.position);

        AudioManager.Instance.PlayOneShot(fireTentacle);
    }

    public virtual List<MoveInput> GetDesiredMovement()
    {
        moveInputs.Clear();
        return moveInputs;
    }

    private void UpdateEndTentaclePoses()
    {
        basePoses[^1] = tentacleHead.position;
        float remainingDist = Vector2.Distance(basePoses[^1], basePoses[^2]);

        while(remainingDist > minDistTentaclesPoints)
        {
            Vector3 dir = basePoses[^1] - basePoses[^2];
            dir.ToV2Dir();

            Vector3 newPoint =  basePoses[^2] + dir * minDistTentaclesPoints;

            AddNewSegment(basePoses[^2], newPoint);

            basePoses.Insert( basePoses.Count - 1, newPoint);
            targetPoses.Insert(targetPoses.Count - 1, newPoint);
            currentPoses.Insert(currentPoses.Count - 1, newPoint);

            remainingDist = Vector2.Distance(basePoses[^1], basePoses[^2]);
        }
    }

    private void UpdateStartTentaclePoses()
    {
        basePoses[0] = root.position;
        float remainingDist = Vector2.Distance(basePoses[0], basePoses[1]);

        while(remainingDist > minDistTentaclesPoints)
        {
            Vector3 dir = basePoses[0] - basePoses[1];
            dir.ToV2Dir();

            Vector3 newPoint =  basePoses[1] + dir * minDistTentaclesPoints;

            AddNewSegment(newPoint, basePoses[1]);

            basePoses.Insert(1, newPoint);
            targetPoses.Insert(1, newPoint);
            currentPoses.Insert(1, newPoint);

            remainingDist = Vector2.Distance(basePoses[0], basePoses[1]);
        }
    }

    private void AdvanceShootDirection(Vector3 expandDir)
    {
        float maxStep = maxAnglePerSecond * Time.deltaTime;
        float angle = Vector2.SignedAngle(shootDir, expandDir);
        shootDir = Quaternion.Euler(0, 0, Mathf.Clamp(angle, -maxStep, maxStep)) * shootDir;
        shootDir.Normalize();
    }

    public Vector3 ComputeExpansionDelta()
    {
        return shootDir * shootSpeed * Time.deltaTime;
    }

    public void UpdateWigglePosesAndTentacleLength()
    {
        float cumulLength = 0f;
        for(int i = 0; i < basePoses.Count-1; i++)
        {
            //we uses dist instead of i cause the FABRIK pass reduce the distance btw points and wiggle gets too noticeable
            cumulLength += Vector2.Distance(basePoses[i], basePoses[i+1]);
         
            float lerpFactor = i * 1f / basePoses.Count;
            float dampFactor = Mathf.Lerp(1f, endDampening, lerpFactor);
            float relativeLength = cumulLength / minDistTentaclesPoints;
            float offset = Mathf.Sin(Time.time * wiggleSpeed + relativeLength * wiggleFrequency) * wiggleAmplitude * dampFactor;

            Vector3 dir = (basePoses[i+1] - basePoses[i]).normalized;
            Vector3 wiggleDir = Vector3.Cross(dir, Vector3.forward);
            targetPoses[i] = basePoses[i] + wiggleDir * offset;
        }
        targetPoses[^1] = basePoses[^1];
        tentacleLength = cumulLength;
    }

    private void UpdateSegments()
    {
        for(int i = 0; i < segments.Count; i++)
        {
            UpdateSegment(currentPoses[i], currentPoses[i+1], segments[i].transform);
        }
    }

    private void UpdateCurrentPoses()
    {
        for(int i = 0; i < currentPoses.Count; i++)
        {
            currentPoses[i] = Vector3.Lerp(currentPoses[i], targetPoses[i], Time.deltaTime * smoothFactor);
        }
    }

    private void AddNewSegment(Vector3 start, Vector3 end)
    {
        GameObject instance = Instantiate(tentacleBodyPrefab, transform.position, Quaternion.identity);

        UpdateSegment(start, end, instance.transform);
        segments.Add(instance);
    }

    public void UpdateSegment(Vector3 start, Vector3 end, Transform segment)
    {
        Vector3 segmentPos = (end + start) / 2;
        Vector2 direction = end - start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle + 90); 
        segment.SetPositionAndRotation(segmentPos, rot);
        
        Vector3 newScale =  segment.localScale;
        newScale.y = Vector2.Distance(end, start) / segment.GetComponent<SpriteRenderer>().sprite.bounds.size.y * 1f;
        segment.transform.localScale = newScale;
    }

    public void ApplyFABRIK(Vector3 target, List<Vector3> points, int iterations, float length = 1)
    {
        if (points.Count < 2) return;
        Vector3 origin = points[0];
        
        for (int iter = 0; iter < iterations; iter++)
        {
            // Forward pass (from end to start)
            points[^1] = target;
            for (int i = points.Count - 2; i >= 0; i--)
            {
                Vector3 direction = (points[i] - points[i + 1]).normalized;
                points[i] = points[i + 1] + direction * length;
            }
            
            // Backward pass (from start to end)
            points[0] = origin;
            for (int i = 1; i < points.Count; i++)
            {
                Vector3 direction = (points[i] - points[i - 1]).normalized;
                points[i] = points[i - 1] + direction * length;
            }
        }
    }

    //bool return if tentacle is fully retracted and can be destroyed so we can end the main process
    public bool RetractAlongPath(Vector3 startPos, out Vector3 endPos)
    {       
        Vector3 currentPos;
        Vector3 nextPos;
        endPos = startPos;
        basePoses[^1]= startPos;

        float remainingDist = retractSpeed * Time.deltaTime;
        int count = 0;

        while(remainingDist > 0f && count <= 20)
        {
            if(basePoses.Count < segmentBeforeDelete || segments.Count <= 2)
            {
                return true;
            }

            count++;
            currentPos = basePoses[^1];
            nextPos = basePoses[^2];

            Vector3 nextPath = nextPos - currentPos;
            if(nextPath.magnitude > remainingDist)
            {
                currentPos += nextPath.normalized * remainingDist;
                remainingDist = 0f;
                basePoses[^1] = currentPos;
            }
            else
            {
                remainingDist -= nextPath.magnitude;
                Destroy(segments[^1]);
                segments.RemoveAt(segments.Count-1);
                basePoses.RemoveAt(basePoses.Count-1);
                targetPoses.RemoveAt(targetPoses.Count-1);
                currentPoses.RemoveAt(currentPoses.Count-1);
            }

            if(count >= 5)
            {
                Debug.LogWarning("Count too high");
            }
        }
        endPos = basePoses[^1];
        return false;
    }
    
    protected void DestroyTentacle()
    {
        OnTentacleDestroyed?.Invoke();
        OnForceRetract?.Invoke(this);

        AudioManager.Instance.PlayOneShot(retractTentacle);
        CleanUp();

        Destroy(tentacleHead.gameObject);
        Destroy(gameObject);
    }

    protected virtual void CleanUp()
    {
        foreach(GameObject e in segments)
        {
            Destroy(e);    
        }
    }

    public virtual void ForceRetract()
    {
        Debug.Log("Force Retracting Tentacle");
        forceRetract = true;
        tentacleHeadHandler.DisableCollider();
        OnForceRetract?.Invoke(this);
    }

    public virtual void HandleHeadCollision(CollisionInfo colInfo){}

    public void SetTargetDir(Vector3 targetDir)
    {
        this.targetDir = targetDir;
    }

    public void SetTargetDirFromPos(Vector3 targetPos)
    {
        Vector3 newTargetDir = targetPos - tentacleHead.position;
        newTargetDir.ToV2Dir();
        targetDir = newTargetDir;
    }

    protected virtual void UpdateRangeStatus()
    {
        if(IsMaxRange(maxTentacleRange))
        {
            if(!hasReachMaxRange && autoRetractOnRangeReach)
            {
                forceRetract = true;
            }
            hasReachMaxRange = true;
        }
        else
        {
            hasReachMaxRange = false;
        }
    }

    private bool IsMaxRange(float maxRange)
    {
        return tentacleLength >= maxRange;
    }


    void OnDrawGizmos()
    {
        if(basePoses.Count > 0)
        {
            for(int i = 0; i < basePoses.Count - 1; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(basePoses[i], basePoses[i+1]);
                
                /*Gizmos.color = Color.blue;
                Gizmos.DrawLine(targetPoses[i], targetPoses[i+1]);
                
                Gizmos.color = Color.red;
                Gizmos.DrawLine(currentPoses[i], currentPoses[i+1]);*/

                Gizmos.DrawWireSphere(root.position, 0.1f);
                Gizmos.DrawWireSphere(tentacleHead.position, 0.1f);
            }
        }       
    }
}
