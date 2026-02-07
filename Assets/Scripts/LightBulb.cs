using UnityEngine;

public class LightBulb : MonoBehaviour, IGrabbable
{
    [SerializeField] private Animator animator;
    [SerializeField] private LightBehavior lightBehavior;
    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Activate()
    {
        animator.SetBool("Activate", true);
        lightBehavior.ActivateLight();
    }

    public void OnGrabStart(Transform grabber)
    {
        transform.parent = grabber;
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void OnGrabEnd(Vector3 grabberVelocity)
    {
        col.enabled = true;
        transform.parent = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(grabberVelocity, ForceMode2D.Impulse);
    }
}
