using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAOE : AreaOfEffect
{
    // Force to apply. Positive to push, negative to pull
    public float force = 0f;

    // Action for this area of effect
    protected override void AreaOfEffectAction()
    {
        List<Unit> enemiesHit = ComputeEnemyTargets();

        foreach(Unit unit in enemiesHit)
        {
            if (force != 0f)
            {
                Vector2 forceDirection = (Vector2)(unit.transform.position - this.transform.position).normalized;
                unit.unitRigidBody.AddForce(force * forceDirection * unit.unitRigidBody.mass, ForceMode2D.Impulse);
            }
        }
    }
}
