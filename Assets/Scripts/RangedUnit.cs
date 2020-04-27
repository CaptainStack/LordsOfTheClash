using UnityEngine;

public class RangedUnit : Unit
{
    // Prefab of the projectile that this unit shoots
    public Projectile projectilePrefab;

    // Accuracy of unit's projectiles (0 - 100 percent)
    public float accuracy = 100f;

    override protected bool CanTargetFlying()
    {
        return true;
    }

    override protected void Attack()
    {
        // Create a projectile and shoot it at the target
        if (projectilePrefab)
        {
            // Calculate target of projectile, with random accuracy adjustments
            Vector3 targetDir = currentTarget.transform.position - this.transform.position;
            float distanceToTarget = (targetDir).magnitude;
            targetDir = targetDir.normalized;

            // Accuracy is 0-100 percent
            accuracy = Mathf.Min(100f, accuracy);
            accuracy = Mathf.Max(0f, accuracy);

            Vector3 normal = Vector3.Cross(targetDir, Vector3.forward);
            targetDir += normal * Random.Range(-100f + accuracy, 100f - accuracy) * .01f;
            targetDir = targetDir.normalized;

            // Cap to max range
            Vector3 target = this.transform.position + targetDir * distanceToTarget;

            // Create projectile
            Projectile newProjectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
            newProjectile.target = target;
            newProjectile.faction = this.faction;

            Mirror.NetworkServer.Spawn(newProjectile.gameObject);
        }
    }
}