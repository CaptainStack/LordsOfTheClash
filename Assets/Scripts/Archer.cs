using UnityEngine;

public class Archer : Unit
{
    public Projectile projectile;

    override protected void Attack()
    {
        // Create a projectile and shoot it at the target
        if (projectile)
        {
            Projectile newProjectile = Instantiate(projectile, this.transform.position, Quaternion.identity);
            newProjectile.target = currentTarget.transform.position;
            newProjectile.faction = this.faction;
        }
    }
}