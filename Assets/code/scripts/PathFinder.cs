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
    [SerializeField] private float stuckRecoveryTime = 1.4f; // nudge toward village if barely moving
    [SerializeField] private float minMoveSpeed = 0.07f;
    private NavMeshAgent agent;
    private Animator animator;
    private bool reachedEnd;
    private bool headingToWanderPoint;
    private float nextRepathTime;
    private float stuckTimer;

    void OnEnable()
    {
        reachedEnd = false;
        headingToWanderPoint = false;
        nextRepathTime = 0f;
        stuckTimer = 0f;
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

        // If the enemy reached its temporary random point, send it back toward the village.
        float randomPointStopDistance = Mathf.Max(agent.stoppingDistance, 0.2f);
        bool reachedRandomPoint = headingToWanderPoint &&
                                  agent.hasPath &&
                                  !agent.pathPending &&
                                  agent.remainingDistance <= randomPointStopDistance;

        if (reachedRandomPoint)
        {
            headingToWanderPoint = false;
            agent.SetDestination(targetPoint.position);
        }

        float stopDistance = Mathf.Max(agent.stoppingDistance, endStopDistance);
        float distanceToVillage = Vector3.Distance(transform.position, targetPoint.position); // world distance, not path length
        if (distanceToVillage <= stopDistance)
        {
            reachedEnd = true;
            PauseMovement(true);
            GameOverFlow.TriggerLose($"{name} reached village");
            return;
        }

        agent.isStopped = false;
        if (Time.time >= nextRepathTime)
        {
            nextRepathTime = Time.time + Random.Range(repathInterval.x, repathInterval.y);
            SetSmartDestination();
        }

        // stuck if path says far away but we barely move (corners etc)
        bool shouldCheckStuck = !agent.pathPending &&
                                agent.hasPath &&
                                agent.remainingDistance > stopDistance + 0.3f;
        bool isHardlyMoving = agent.velocity.sqrMagnitude < minMoveSpeed * minMoveSpeed;

        if (shouldCheckStuck && isHardlyMoving)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckRecoveryTime)
            {
                stuckTimer = 0f;
                headingToWanderPoint = false;
                agent.ResetPath();
                agent.SetDestination(targetPoint.position);
                nextRepathTime = Time.time + 0.2f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        if (animator != null)
        {
            if (agent.velocity.sqrMagnitude > 0.01f)
                animator.speed = 1f;
            else
                animator.speed = 0f;
        }
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

        if (resetPath)
        {
            headingToWanderPoint = false;
            stuckTimer = 0f;
        }

        if (animator != null) animator.speed = 0f;
    }

    private void SetSmartDestination()
    {
        if (targetPoint == null || agent == null)
            return;

        if (Random.value >= wanderChance) // usually aim at village
        {
            headingToWanderPoint = false;
            agent.SetDestination(targetPoint.position);
            return;
        }
        
        float currentToGoal = Vector3.Distance(transform.position, targetPoint.position);
        Vector3 forwardSample = Vector3.Lerp(transform.position, targetPoint.position, 0.4f);

        for (int i = 0; i < 4; i++)
        {
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = forwardSample + new Vector3(offset.x, 0f, offset.y);
            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
                continue;
            if (Vector3.Distance(hit.position, targetPoint.position) >= currentToGoal)
                continue;

            NavMeshPath path = new NavMeshPath();
            bool hasCompletePath = agent.CalculatePath(hit.position, path) &&
                                   path.status == NavMeshPathStatus.PathComplete;

            if (hasCompletePath)
            {
                headingToWanderPoint = true;
                agent.SetDestination(hit.position);
                return;
            }
        }
        
        headingToWanderPoint = false;
        agent.SetDestination(targetPoint.position);
    }
}