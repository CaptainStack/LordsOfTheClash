using UnityEngine;

public class Zombie : Unit
{
    public float attackDamage = 1f;

    override protected void Attack()
    {
        currentTarget.health -= attackDamage;
    }
}