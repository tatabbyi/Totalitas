using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinder : MonoBehaviour
{
    public Transform targetPoint; // Point B
    [SerializeField] private MazePlacementManager placementManager;
    [SerializeField] private float endStopDistance = 1.2f;
    [SerializeField, Range(0f, 1f)] private float wanderChance = 0.22f;
    [SerializeField] private float wanderRadius = 2f;
    [SerializeField] private Vector2 repathInterval = new Vector2(0.5f, 1f);
    private NavMeshAgent agent;
    private Animator animator;
    private bool reachedEnd;
    private float nextRepathTime;

    void OnEnable()
    {
        reachedEnd = false;
        nextRepathTime = 0f;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (placementManager == null) placementManager = FindFirstObjectByType<MazePlacementManager>();
        if (animator != null) animator.applyRootMotion = false;
        PauseMovement(true);

        if (targetPoint == null)
            Debug.LogError("No target point assigned for EnemyPathfinder!");
    }

    void Update()
    {
        if (agent == null)
            return;

        if (placementManager == null) placementManager = FindFirstObjectByType<MazePlacementManager>();
        if (placementManager == null || !placementManager.RunStarted)
        {
            reachedEnd = false;
            PauseMovement(true);
            return;
        }

        if (targetPoint == null)
        {
            PauseMovement();
            return;
        }

        if (reachedEnd)
        {
            PauseMovement();
            return;
        }

        float stopDistance = Mathf.Max(agent.stoppingDistance, endStopDistance);
        if (agent.hasPath && !agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            reachedEnd = true;
            PauseMovement(true);
            GameOverFlow.Trigger($"{name} reached village");
            return;
        }

        agent.isStopped = false;
        if (Time.time >= nextRepathTime)
        {
            nextRepathTime = Time.time + Random.Range(repathInterval.x, repathInterval.y);
            SetSmartDestination();
        }

        if (animator != null) animator.speed = agent.velocity.sqrMagnitude > 0.01f ? 1f : 0f;
    }

    public void FreezeNow()
    {
        reachedEnd = true;
        PauseMovement(true);
        enabled = false;
    }

    private void PauseMovement(bool resetPath = false)
    {
        if (agent != null)
        {
            agent.isStopped = true;
            if (resetPath) agent.ResetPath();
        }

        if (animator != null) animator.speed = 0f;
    }

    private void SetSmartDestination()
    {
        if (targetPoint == null || agent == null) return;
        if (Random.value >= wanderChance) { agent.SetDestination(targetPoint.position); return; }
        
        float currentToGoal = Vector3.Distance(transform.position, targetPoint.position);
        Vector3 forwardSample = Vector3.Lerp(transform.position, targetPoint.position, 0.4f);

        for (int i = 0; i < 4; i++)
        {
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = forwardSample + new Vector3(offset.x, 0f, offset.y);
            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas)) continue;
            if (Vector3.Distance(hit.position, targetPoint.position) >= currentToGoal) continue;

            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path) && path.status != NavMeshPathStatus.PathInvalid) { agent.SetDestination(hit.position); return; }
        }
        
        agent.SetDestination(targetPoint.position);
    }
}