using System.Collections.Generic;
using UnityEngine;

public class MoveTentacle : Tentacle
{
    [SerializeField] private float initialMoveStrenght;
    [SerializeField] private float moveStrenghtOverTime;

    private bool isGrabbing = false;
    private bool firstImpact = true;

    protected override void ApplyChildVisuals()
    {
        if(isGrabbing)
        {
            if(Vector2.Distance(root.position, tentacleHead.position) < 0.5f)
            {
                DestroyTentacle();
                return;
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
                moveInputs.Add(new MoveInput(dirToRoot * initialMoveStrenght, MoveType.Impulse));
                firstImpact = false;
            }

            moveInputs.Add(new MoveInput(dirToRoot * moveStrenghtOverTime, MoveType.Velocity));
        }

        moveInputs.Add(new MoveInput(Vector3.zero, MoveType.Velocity));
        return moveInputs;
    }

    public override void HandleHeadCollision(Collision2D collision)
    {
        if(collision.transform.CompareTag("Wall"))
        {
            isGrabbing = true;
            applyForces = false;
            forceExpand = false;
            canExpand = false;
            wiggleAmplitude /= 2f;
        }
    }
}
