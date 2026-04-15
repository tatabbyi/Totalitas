using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinder : MonoBehaviour
{
    public Transform targetPoint; // Point B
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (targetPoint != null)
        {
            agent.SetDestination(targetPoint.position);
        }
        else
        {
            Debug.LogError("No target point assigned for EnemyPathfinder!");
        }
    }

    void Update()
    {
        if (targetPoint != null)
        {
            // Update destination continuously in case target moves
            agent.SetDestination(targetPoint.position);
        }
    }
}