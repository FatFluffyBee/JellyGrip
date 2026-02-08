using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour, IMoveReceiver
{
    //for test purpose only
    [SerializeField] private List<IMoveGiver> moveGivers = new List<IMoveGiver>();
    [SerializeField] private float decayRate;
    [SerializeField] private float buildUpRate;
    [SerializeField] private float maxSpeed;
    private Vector3 currentVelocity;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        currentVelocity = rb.linearVelocity;

        Vector3 newVelocity = Vector3.zero;
        foreach(IMoveGiver e in moveGivers)
        {
            MoveInput moveInput = e.GetDesiredMovement();

            switch(moveInput.moveType)
            {
                case MoveType.Velocity:
                    newVelocity += moveInput.input;
                    break;
                
                case MoveType.Impulse:
                    currentVelocity += moveInput.input;
                    break;
            }
        }

        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);

        if(currentVelocity.sqrMagnitude < newVelocity.sqrMagnitude)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, newVelocity, Time.deltaTime * buildUpRate);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, newVelocity, Time.deltaTime * decayRate);
        }

        rb.linearVelocity = currentVelocity;
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
}

public enum MoveType {Velocity, Impulse, Override}
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
