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
    private NavMeshAgent agent;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        
        agent = GetComponent<NavMeshAgent>();
        
        if (targetPoint == null)
        {
            Debug.LogError("MazeEnemy on " + gameObject.name + " needs a targetPoint assigned!");
            agent.enabled = false;
            return;
        }
        
        if (agent == null)
        {
            Debug.LogError("MazeEnemy needs a NavMeshAgent component!");
            return;
        }
        
        agent.SetDestination(targetPoint.position);
        Debug.Log(gameObject.name + " pathfinding to " + targetPoint.name);
    }

    void Update()
    {
        if (targetPoint != null && agent.enabled)
        {
            agent.SetDestination(targetPoint.position);
        }
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
