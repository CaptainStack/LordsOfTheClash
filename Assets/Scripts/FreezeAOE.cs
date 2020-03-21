using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAOE : AreaOfEffect
{
    // Duration of the freeze
    public float duration = 1f;

    // Action for this area of effect
    protected override void AreaOfEffectAction()
    {
        List<Unit> enemiesHit = ComputeEnemyTargets();

        foreach(Unit unit in enemiesHit)
        {
            FreezeUnitEffect freezeUnitEffect = unit.gameObject.AddComponent<FreezeUnitEffect>();
            freezeUnitEffect.duration = duration;
        }
    }
}
