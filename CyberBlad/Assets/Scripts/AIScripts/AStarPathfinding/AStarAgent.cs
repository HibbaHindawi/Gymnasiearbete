using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour
{
    public Transform target;
    private int targetIndex;
    private Vector3 targetOldPosition;

    public float searchRadius = 10f; // Maximum radius to search for an endpoint
    public float speed = 10f;
    public int rotationSpeed = 3;

    private Vector3[] path;
    public float pathUpdateDelay = 0.3f; // Delay between path updates

    private bool isUpdatingPath = false;        // Flag to prevent multiple path update requests
    private bool isFollowingPath = false;       // Flag to track if the agent is currently following a path
    public bool IsFollowingPath => isFollowingPath;

    private bool isFollowingPlayer = false; // Flag to track if the agent is following the player

    public delegate void PathCompletedHandler();
    public event PathCompletedHandler OnPathCompleted; // Event to signal path completion

    private void Awake()
    {
        targetOldPosition = transform.position; // Set initial targetOldPosition to agent's position
    }

    private void Update()
    {
        if (target != null && !isFollowingPlayer && Vector3.Distance(targetOldPosition, target.position) > 3f && !isUpdatingPath && !isFollowingPath)
        {
            StartCoroutine(UpdatePathWithDelay());
        }
    }

    private IEnumerator UpdatePathWithDelay()
    {
        isUpdatingPath = true;

        // Calculate a random endpoint within the search radius
        Vector3 randomOffset = Random.insideUnitSphere * searchRadius;
        Vector3 randomTargetPosition = target.position + randomOffset;

        if (target.CompareTag("Player")) // Check if the target is the player
        {
            pathUpdateDelay = 0.1f; // Set path update delay to 0.1 if the target is the player
        }

        PathRequestManager.RequestPath(transform.position, randomTargetPosition, OnPathFound);
        targetOldPosition = target.position;

        yield return new WaitForSeconds(pathUpdateDelay);

        isUpdatingPath = false;
    }

    public void OnPathFound(Vector3[] newPath, bool pathIsSuccessful)
    {
        if (pathIsSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StartCoroutine(FollowPath());
        }
    }

    private IEnumerator FollowPath()
    {
        isFollowingPath = true;

        while (targetIndex < path.Length)
        {
            Vector3 currentWaypoint = path[targetIndex];

            while (transform.position != currentWaypoint)
            {
                Vector3 targetDir = currentWaypoint - transform.position;
                float step = rotationSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
                transform.rotation = Quaternion.LookRotation(newDir);

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

                yield return null;
            }

            targetIndex++;
        }

        isFollowingPath = false;

        // Invoke the OnPathCompleted event
        OnPathCompleted?.Invoke();
    }

    public void SetTarget(Transform newTarget)
    {
        if (!isFollowingPlayer) 
        {
            target = newTarget;
            targetOldPosition = transform.position; // Update the targetOldPosition when setting a new target
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);

                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}