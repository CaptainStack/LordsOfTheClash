using UnityEngine;

public class MeleeUnit : Unit
{
    public float attackDamage = 1f;

    private int flyingLayer;

    void Awake()
    {
        flyingLayer = LayerMask.NameToLayer("Flying");
    }

    override protected bool CanTargetFlying()
    {
        return this.gameObject.layer == flyingLayer;
    }

    override protected void Attack()
    {
        currentTarget.health -= attackDamage;
    }
}