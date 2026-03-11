using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour, IMoveGiver, IPushable
{
    private Vector3 currentKnockback;

    private List<MoveInput> moveInputs = new List<MoveInput>();

    private void Awake()
    {
        GetComponentInParent<IMoveReceiver>()?.AddMovementSource(this);
    }

    public void Push(Vector3 pushForce)
    {
        currentKnockback += pushForce;
    }

    public List<MoveInput> CalculateMovementToGive(MoveReceiverData moveReceiverData)
    {
        Vector3 tmp = currentKnockback;
        currentKnockback = Vector3.zero;
        
        moveInputs.Clear();
        moveInputs.Add(new MoveInput(tmp, MoveType.Impulse));
        return moveInputs;
    }
}
