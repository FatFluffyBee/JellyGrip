using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour, IMoveReceiver
{
    //for test purpose only
    

    [Header("Constraints")]
    [SerializeField] private DecayMode decayMode;
    [SerializeField] private float velocityDecayRate;
    [SerializeField] private float maxSpeed;

    [Header("Debug")]
    [SerializeField] private float debugIntensity;

    private Vector3 currentVelocity;
    private List<IMoveGiver> moveGivers = new List<IMoveGiver>();

    private Rigidbody2D rb;
    private enum DecayMode {Linear, Exponential};

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        currentVelocity = rb.linearVelocity;

        MoveIntent moveIntent = GatherMoveGiverInput();
        AddMoveIntent(moveIntent);
        ApplyDecay();
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        rb.linearVelocity = currentVelocity;
    }

    private MoveIntent GatherMoveGiverInput()
    {
        DisplayDebugLine(currentVelocity, Color.yellow);

        MoveIntent forces = new MoveIntent();

        foreach(IMoveGiver moveGiver in moveGivers)
        {
            foreach(MoveInput e in moveGiver.GetDesiredMovement())
            {
                switch(e.moveType)
                {
                    case MoveType.Acceleration:
                        DisplayDebugLine(e.input, Color.green);
                        forces.acceleration += e.input;
                        break;
                    
                    case MoveType.Impulse:
                        DisplayDebugLine(e.input, Color.red);
                        forces.velocity += e.input;
                        break;
                }
            }
        }
        return forces;
    } 

    private void AddMoveIntent(MoveIntent moveIntent)
    {
        currentVelocity += moveIntent.velocity;
        currentVelocity += moveIntent.acceleration * Time.deltaTime;
    }
   
    private void ApplyDecay()
    {
        switch(decayMode)
        {
            case DecayMode.Linear:
                currentVelocity *= 1 - velocityDecayRate * Time.deltaTime;
                break;

            case DecayMode.Exponential:
                currentVelocity *= Mathf.Exp(-velocityDecayRate * Time.deltaTime);
                break;
        }
    }

    public void AddMovementSource(IMoveGiver giver)
    {
        if(moveGivers.Contains(giver))
            return;

        moveGivers.Add(giver);
    }

    public void RemoveMovementSource(IMoveGiver giver)
    {
        if(moveGivers.Contains(giver))
        {
            moveGivers.Remove(giver);
        }
    }

    private void DisplayDebugLine(Vector3 moveVector, Color color)
    {
        Debug.DrawLine(transform.position, transform.position + moveVector * debugIntensity, color);
    }

    private struct MoveIntent
    {
        public Vector3 acceleration;
        public Vector3 velocity;

        public MoveIntent(Vector3 acceleration, Vector3 velocity)
        {
            this.acceleration = acceleration;
            this.velocity = velocity;
        }
    }
}

public enum MoveType {Acceleration, Impulse, Override}
public struct MoveInput
{
    public Vector3 input;
    public MoveType moveType;

    public MoveInput(Vector3 input, MoveType moveType)
    {
        this.input = input;
        this.moveType = moveType;
    }
}
