﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmAOE : AreaOfEffect
{
    // Duration of the charm
    public float duration = 1f;

    // Action for this area of effect
    protected override void AreaOfEffectAction()
    {
        List<Unit> enemiesHit = ComputeEnemyTargets();

        foreach(Unit unit in enemiesHit)
        {
            CharmUnitEffect charmUnitEffect = unit.gameObject.AddComponent<CharmUnitEffect>();
            charmUnitEffect.duration = duration;
            charmUnitEffect.faction = faction;
        }
    }
}
