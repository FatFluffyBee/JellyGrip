using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour, IMoveGiver
{
    private Vector3 currentKnockback;

    private List<MoveInput> moveInputs = new List<MoveInput>();

    public void Apply(Vector3 colPoint, float knockbackForce)
    {
        Vector3 knockBackDir = (transform.position - colPoint).normalized;
        knockBackDir.ToV2Dir();
        knockBackDir *= knockbackForce;

        currentKnockback += knockBackDir;
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
