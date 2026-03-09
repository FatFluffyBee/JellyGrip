using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbPlatform;
    [SerializeField] private List<Transform> wayPoints = new List<Transform>();
    [SerializeField] private float speed = 2f;
    [SerializeField] private float thresholdDistance = 0.1f;
    [SerializeField] private bool showDebug = false;

    private Vector3 currentTarget;
    private int currentTargetIndex;

    void Awake()
    {
        if(wayPoints.Count < 2)
        {
            Debug.LogError("Not enough waypoints assigned to the moving platform.");
            enabled = false;
            return;
        }

        rbPlatform.MovePosition(wayPoints[0].position);
        currentTarget = wayPoints[1].position;
        currentTargetIndex = 1;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(rbPlatform.position, currentTarget) < thresholdDistance)
        {
            currentTargetIndex = GetNextIndex();
            currentTarget = wayPoints[currentTargetIndex].position;
        }

        Vector3 newPos = Vector3.MoveTowards(rbPlatform.position, currentTarget, speed * Time.fixedDeltaTime);

        rbPlatform.MovePosition(newPos);
    }

    private int GetNextIndex()
    {
        return currentTargetIndex + 1 >= wayPoints.Count ? 0 : currentTargetIndex + 1;
    }


    private void OnDrawGizmos()
    {
        if(showDebug == false || wayPoints.Count < 2)
            return;
            
        if(Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTarget, 0.1f);
        }
        
        Color startColor = Color.green;
        Color endColor = Color.blue;

        for(int i = 0; i < wayPoints.Count; i++)
        {
            Gizmos.color = Color.Lerp(startColor, endColor, i / (wayPoints.Count - 1f));
            if (i < wayPoints.Count - 1)
            {
                Gizmos.DrawLine(wayPoints[i].position, wayPoints[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(wayPoints[i].position, wayPoints[0].position);
            }
        }
    }
}
