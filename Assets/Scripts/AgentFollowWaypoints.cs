using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Collections.Generic;

public class AgentFollowWaypoints : MonoBehaviour
{
    public List<Transform> waypoints;    // Assign waypoints in the Inspector
    
    public Animator animator;

    public NavMeshAgent navMeshAgent;

    public bool startFollowing = false; // Set to true to start following
    
    private int currentWaypointIndex = 0;

    void Start()
    {
        PrepComponent();

        if (!startFollowing)
        {
            navMeshAgent.enabled = false;
        }
        else
        {
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void OnValidate()
    {
        PrepComponent();
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

            if (waypoints.Count == 0) return;

            // Check if agent has reached the destination
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Count)
                {
                    Destroy(gameObject); // Delete agent when done
                    return;
                }

                navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
            }

            // Animation handling
            if (animator != null)
            {
                float speedPercent = (navMeshAgent.velocity.magnitude / navMeshAgent.speed) + 0.1f;
                animator.SetFloat("SpeedPercent", speedPercent);
                Debug.Log(speedPercent);
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
    
    void PrepComponent()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.gameObject.GetComponent<Animator>() == null)
            {
                children.Add(child);
            }
        }

        waypoints = children;
    }
}