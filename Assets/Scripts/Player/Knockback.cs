using System.Collections.Generic;
using UnityEngine;

//! it's a bit weird cause act as a buffer between movement and IPushable
//It might be good for movement to add a small queue of movement impulse that get cleared after
public class Knockback : MonoBehaviour, IMoveGiver
{
    private Vector3 currentKnockback;

    private List<MoveInput> moveInputs = new List<MoveInput>();

    public void Apply(Vector3 dir, float knockbackForce)
    {
        
        currentKnockback += dir * knockbackForce;
    }

    public List<MoveInput> GetDesiredMovement()
    {
        Vector3 tmp = currentKnockback;
        currentKnockback = Vector3.zero;
        
        moveInputs.Clear();
        moveInputs.Add(new MoveInput(tmp, MoveType.Impulse));
        return moveInputs;
    }
}
