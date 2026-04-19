using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private int damagePerTick = 15;
    [SerializeField] private float tickInterval = 1.0f;

    private readonly Dictionary<Component, float> _nextDamageTime = new();

    private void OnTriggerEnter(Collider other)
    {
        TryApplyDamage(other, true);
    }

    private void OnTriggerStay(Collider other)
    {
        TryApplyDamage(other, false);
    }

    private void TryApplyDamage(Collider other, bool forceApply)
    {
        if (!other.CompareTag("Enemy")) return;

        Component damageable = GetDamageable(other);
        if (damageable == null) return;

        float now = Time.time;
        if (!_nextDamageTime.ContainsKey(damageable))
            _nextDamageTime[damageable] = now;

        if (forceApply || now >= _nextDamageTime[damageable])
        {
            ApplyDamage(damageable);
            TriggerEnemyHurtAnimation(other);
            _nextDamageTime[damageable] = now + tickInterval;
        }
    }

    private Component GetDamageable(Collider other)
    {
        EnemyHealth health = other.GetComponent<EnemyHealth>();
        if (health != null) return health;

        MazeEnemy mazeEnemy = other.GetComponent<MazeEnemy>();
        if (mazeEnemy != null) return mazeEnemy;

        return null;
    }

    private void ApplyDamage(Component damageable)
    {
        if (damageable is EnemyHealth health)
        {
            health.TakeDamage(damagePerTick);
        }
        else if (damageable is MazeEnemy mazeEnemy)
        {
            mazeEnemy.TakeDamage(damagePerTick);
        }
    }

    private void TriggerEnemyHurtAnimation(Collider other)
    {
        Animator animator = other.GetComponent<Animator>();
        if (animator == null)
            animator = other.GetComponentInChildren<Animator>();
        if (animator == null) return;

        animator.SetBool("isHurt", true);
        StartCoroutine(ResetHurt(animator));
    }

    private System.Collections.IEnumerator ResetHurt(Animator animator)
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
            animator.SetBool("isHurt", false);
    }

    private void OnTriggerExit(Collider other)
    {
        Component damageable = GetDamageable(other);
        if (damageable != null) _nextDamageTime.Remove(damageable);
    }
}