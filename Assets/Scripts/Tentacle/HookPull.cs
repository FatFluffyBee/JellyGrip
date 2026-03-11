using System.Collections.Generic;
using UnityEngine;

public class HookPull : IMoveGiver
{
    private HookPullConfig config;
    private Transform pulledObject; // ( )------->(x)
    private Transform anchorObject; // (x)------->( )

    private bool isFirstFrame;

    public HookPull(Transform pulledObject, Transform anchorObject, HookPullConfig hookPullConfig)
    {
        this.pulledObject = pulledObject;
        this.anchorObject = anchorObject;
        config = hookPullConfig;
        isFirstFrame = true;
    }

    public List<MoveInput> CalculateMovementToGive(MoveReceiverData moveReceiverData)
    {
        List<MoveInput> moveInputs = new List<MoveInput>();
       
            Vector2 pullToAnchor = anchorObject.position - pulledObject.position;
            Vector2 pullDir = pullToAnchor.normalized;

            if(isFirstFrame)
            {
                moveInputs.Add(new MoveInput(pullDir * config.initialPullForce, MoveType.Impulse));
                isFirstFrame = false;
            }

            float pullForce = CalculatePullForce(pullToAnchor.magnitude);
            moveInputs.Add(new MoveInput(pullDir * pullForce, MoveType.Acceleration));

        moveInputs.Add(new MoveInput(Vector3.zero, MoveType.Acceleration));
        return moveInputs;
    }

     private float CalculatePullForce(float distance)
    {
        float t = Mathf.InverseLerp(config.minRangePull, config.maxRangePull, distance);
        t = Mathf.Clamp01(t);
        t = 1 - (1 - t) * (1 - t) * (1 - t);
        float force = Mathf.Lerp(config.minConstantPullForce, config.maxConstantPullForce, t);
        return force;
    }
}

public struct HookPullConfig
{
    public float initialPullForce;
    public float maxConstantPullForce;
    public float minConstantPullForce;
    public float maxRangePull;
    public float minRangePull;
}
