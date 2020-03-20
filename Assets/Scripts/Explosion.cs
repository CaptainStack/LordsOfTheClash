using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : AreaOfEffect
{
    public float damage = 1f;
    public float impactForce = 0f;

    // Action for this area of effect
    protected override void AreaOfEffectAction()
    {
        List<Unit> enemiesHit = ComputeEnemyTargets();

        foreach(Unit unit in enemiesHit)
        {
            unit.health -= damage;

            if (impactForce > 0f)
            {
                Vector3 impactForceDirection = (unit.transform.position - this.transform.position).normalized;
                unit.unitRigidBody.AddForce(impactForce * impactForceDirection, ForceMode2D.Impulse);
            }
        }
    }
}
