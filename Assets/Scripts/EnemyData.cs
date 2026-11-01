using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Combat/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Identity")]
    public string enemyType = "Small Bird";

    [Header("Combat Stats")]
    [Tooltip("Health points")]
    public int maxHealth = 1;

    [Tooltip("Damage points")]
    public int contactDamage = 1;

    [Header("Visual Effects")]
    [Tooltip("VFX Death")]
    public GameObject explosionVfxPrefab;
}