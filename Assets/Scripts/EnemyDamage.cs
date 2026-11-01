using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Data Profile")]
    [SerializeField] private EnemyData enemyData;

    [Header("Combat Settings")]
    [Tooltip("Damage Amount")]
    [SerializeField] private int damageAmount = 1;

    public int DamageAmount => enemyData != null ? enemyData.contactDamage : damageAmount;
}