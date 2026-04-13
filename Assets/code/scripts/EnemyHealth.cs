using UnityEngine;

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
            Destroy(gameObject);
        }
    }
}