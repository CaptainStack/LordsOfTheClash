using UnityEngine;

public class MeleeUnit : Unit
{
    public float attackDamage = 1f;

    override protected void Attack()
    {
        currentTarget.health -= attackDamage;
    }
}