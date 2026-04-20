using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MazeEnemy : MonoBehaviour
{
    [Header("Health System")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("Health Bar UI")]
    public Image healthBarFill;
    public Text healthText;
    
    [Header("Pathfinding")]
    public Transform targetPoint;
    [SerializeField] private MazePlacementManager placementManager;
    [SerializeField] private float snapToNavMeshDistance = 50f;
    [Header("Movement Variation")]
    [SerializeField, Range(0f, 1f)] private float detourChance = 0.25f;
    [SerializeField] private float detourRadius = 5f;
    [SerializeField] private Vector2 decisionIntervalRange = new Vector2(0.9f, 1.6f);
    [SerializeField] private Vector2 detourDurationRange = new Vector2(1.2f, 2.4f);
    [SerializeField] private int detourSampleAttempts = 6;

    private NavMeshAgent agent;
    private bool isDetouring;
    private float nextDecisionTime;
    private float detourEndTime;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("MazeEnemy needs a NavMeshAgent component!");
            return;
        }

        if (placementManager == null)
            placementManager = FindFirstObjectByType<MazePlacementManager>();

        if (targetPoint == null)
        {
            GameObject taggedTarget = GameObject.FindWithTag("VillageTarget");
            if (taggedTarget != null)
                targetPoint = taggedTarget.transform;
            else
            {
                GameObject namedTarget = GameObject.Find("VillageTarget");
                if (namedTarget != null)
                    targetPoint = namedTarget.transform;
            }
        }

        if (targetPoint == null)
        {
            Debug.LogError("MazeEnemy on " + gameObject.name + " needs a targetPoint assigned or a VillageTarget tagged object!");
            agent.enabled = false;
            return;
        }

        SnapToNavMesh();

        agent.isStopped = true;
        nextDecisionTime = Time.time;
        Debug.Log(gameObject.name + " ready to pathfind to " + targetPoint.name);
    }

    void Update()
    {
        if (targetPoint != null && agent.enabled)
        {
            if (!agent.isOnNavMesh)
            {
                SnapToNavMesh();
                return;
            }

            if (placementManager != null && !placementManager.RunStarted) // wait for start wave
            {
                agent.isStopped = true;
                return;
            }

            agent.isStopped = false;
            UpdateDestination();
        }
    }

    void SnapToNavMesh()
    {
        if (agent == null)
            return;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, snapToNavMeshDistance, NavMesh.AllAreas))
            agent.Warp(hit.position);
    }

    private void UpdateDestination()
    {
        if (agent == null || targetPoint == null)
            return;

        if (isDetouring && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
            isDetouring = false;

        if (Time.time < nextDecisionTime)
            return;

        nextDecisionTime = Time.time + Random.Range(decisionIntervalRange.x, decisionIntervalRange.y);

        if (isDetouring && Time.time >= detourEndTime)
            isDetouring = false;

        if (!isDetouring && Random.value < detourChance && TryGetDetourPoint(out Vector3 detourPoint))
        {
            isDetouring = true;
            detourEndTime = Time.time + Random.Range(detourDurationRange.x, detourDurationRange.y);
            agent.SetDestination(detourPoint);
            return;
        }

        agent.SetDestination(targetPoint.position);
    }

    private bool TryGetDetourPoint(out Vector3 detourPoint) // random nav point, not too far from goal
    {
        detourPoint = Vector3.zero;

        for (int i = 0; i < detourSampleAttempts; i++)
        {
            Vector2 circle = Random.insideUnitCircle * detourRadius;
            Vector3 candidate = transform.position + new Vector3(circle.x, 0f, circle.y);

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, detourRadius, NavMesh.AllAreas))
                continue;

            float currentDistanceToGoal = Vector3.Distance(transform.position, targetPoint.position);
            float detourDistanceToGoal = Vector3.Distance(hit.position, targetPoint.position);

            if (detourDistanceToGoal <= currentDistanceToGoal + detourRadius * 1.5f)
            {
                detourPoint = hit.position;
                return true;
            }
        }

        return false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log(gameObject.name + " took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
        if (healthText != null)
        {
            healthText.text = currentHealth + "/" + maxHealth;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        agent.enabled = false;
        Destroy(gameObject, 1f);
    }
}
