using UnityEngine;
using UnityEngine.AI;

public class AgentFollowWaypoints : MonoBehaviour
{
    public Transform[] waypoints;    // Assign waypoints in the Inspector
    public bool startFollowing = false; // Set to true to start following
    public Animator animator;

    private NavMeshAgent navMeshAgent;
    private int currentWaypointIndex = 0;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (!startFollowing)
        {
            navMeshAgent.enabled = false;
        }
        else
        {
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (startFollowing)
        {
            if (!navMeshAgent.enabled)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
            }

            if (waypoints.Length == 0) return;

            // Check if agent has reached the destination
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    Destroy(gameObject); // Delete agent when done
                    return;
                }

                navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
            }

            // Animation handling
            if (animator != null)
            {
                float speedPercent = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
                animator.SetFloat("SpeedPercent", speedPercent);
            }
        }
        else
        {
            // If not following, ensure the idle animation plays
            if (animator != null)
            {
                animator.SetFloat("SpeedPercent", 0f);
            }
        }
    }
    
}