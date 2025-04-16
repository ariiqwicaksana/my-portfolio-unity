using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrol : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    public Transform[] patrolPoints;
    public int targetPoint;
    public float speed;
    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        targetPoint = 0;
        transform.position = patrolPoints[targetPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == patrolPoints[targetPoint].position)
        {
            increaseTargetInt();
            // Cast a ray downward to find the ground point directly below the next patrol point
            ray = new Ray(patrolPoints[targetPoint].position + Vector3.up * 10, Vector3.down);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
            {
                patrolPoints[targetPoint].position = hit.point;
            }
            // Rotate towards the next patrol point
            Vector3 direction = patrolPoints[targetPoint].position - transform.position;
            direction.y = 0; // Keep the rotation horizontal
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        // Move towards the target point
        if ((patrolPoints[targetPoint].position - transform.position).sqrMagnitude < 0.02f)
        {
            transform.position = patrolPoints[targetPoint].position;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, speed * Time.deltaTime);
        }
    }

    void increaseTargetInt()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }
}
