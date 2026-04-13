using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private int damagePerTick = 15;
    [SerializeField] private float tickInterval = 1.0f;

    private readonly Dictionary<EnemyHealth, float> _nextDamageTime = new();

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHealth health = other.GetComponent<EnemyHealth>();
        if (health == null) return;

        float now = Time.time;
        if (!_nextDamageTime.ContainsKey(health))
            _nextDamageTime[health] = now;

        if (now >= _nextDamageTime[health])
        {
            health.TakeDamage(damagePerTick);
            _nextDamageTime[health] = now + tickInterval;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyHealth health = other.GetComponent<EnemyHealth>();
        if (health != null) _nextDamageTime.Remove(health);
    }
}