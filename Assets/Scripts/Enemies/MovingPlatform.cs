using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float speed;

    private Vector3 target;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        target = endPoint.position;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = target == startPoint.position ? endPoint.position : startPoint.position;
        }

        Vector3 newPos = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);
    }
}
