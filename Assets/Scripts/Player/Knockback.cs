using UnityEngine;

public class Knockback : MonoBehaviour, IMoveGiver
{
    private Vector3 currentKnockback;

    public void Apply(Vector3 colPoint, float knockbackForce)
    {
        Vector3 knockBackDir = (transform.position - colPoint).normalized;
        knockBackDir.ToV2Dir();
        knockBackDir *= knockbackForce;

        currentKnockback += knockBackDir;
    }

    public MoveInput GetDesiredMovement()
    {
        Vector3 tmp = currentKnockback;
        currentKnockback = Vector3.zero;
        return new MoveInput(tmp, MoveType.Impulse);
    }
}
