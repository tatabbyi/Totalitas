using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. HP = {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Debug.Log($"{name} died.");
            var pathfinder = GetComponent<EnemyPathfinder>();
            if (pathfinder != null) pathfinder.FreezeNow();

            var agent = GetComponent<NavMeshAgent>();
            if (agent != null) { agent.isStopped = true; agent.ResetPath(); }

            var animator = GetComponent<Animator>();
            if (animator != null) animator.speed = 0f;

            Destroy(gameObject, 0.15f);
        }
    }
}