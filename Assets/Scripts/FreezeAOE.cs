using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAOE : AreaOfEffect
{
    public FreezeUnitEffect freezeUnitEffect;

    // Action for this area of effect
    protected override void AreaOfEffectAction()
    {
        if (freezeUnitEffect)
        {
            List<Unit> enemiesHit = ComputeEnemyTargets();

            foreach(Unit unit in enemiesHit)
            {
                FreezeUnitEffect freezeEffect = Instantiate(freezeUnitEffect, unit.transform.position, Quaternion.identity);
                unit.ApplyEffect(freezeEffect);
            }
        }
    }
}
