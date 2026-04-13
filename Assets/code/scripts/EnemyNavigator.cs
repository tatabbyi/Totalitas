using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigator : MonoBehaviour
{
    [SerializeField] private Transform villageTarget;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (villageTarget != null)
            _agent.SetDestination(villageTarget.position);
        else
            Debug.LogWarning($"{name}: Village target not assigned.");
    }
}