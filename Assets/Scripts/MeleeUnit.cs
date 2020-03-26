using UnityEngine;

public class MeleeUnit : Unit
{
    public float attackDamage = 1f;

    override protected bool CanTargetFlying()
    {
        return this.transform.position.z == -1 ? true : false;
    }

    override protected void Attack()
    {
        currentTarget.health -= attackDamage;
    }
}