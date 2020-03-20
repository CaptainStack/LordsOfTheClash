using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Freeze debuff for a unit
public class FreezeUnitEffect : UnitEffect
{

    // What to do at the start of the effect
    protected override void OnEffectStart()
    {
        // Apply freeze to target by disabling its AI
        target.DisableAI();
    }

    // What to do at the end of the effect
    protected override void OnEffectEnd()
    {
        // Remove freeze from target by resuming its AI
        target.ResumeAI();
    }
}
