using UnityEngine;

public class RangedUnit : Unit
{
    public Projectile projectilePrefab;

    override protected void Attack()
    {
        // Create a projectile and shoot it at the target
        if (projectilePrefab)
        {
            Projectile newProjectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
            newProjectile.target = currentTarget.transform.position;
            newProjectile.faction = this.faction;
        }
    }
}