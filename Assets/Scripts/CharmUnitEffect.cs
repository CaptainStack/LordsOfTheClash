﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Charm debuff for a unit
public class CharmUnitEffect : UnitEffect
{
    public Faction faction;
    private Faction prevFaction;

    // What to do at the start of the effect
    protected override void OnEffectStart()
    {
        // New charm effect overrides any previous hostile charm, or any same-faction charm that expires sooner
        foreach (CharmUnitEffect charm in unit.gameObject.GetComponents<CharmUnitEffect>())
        {
            if (charm != this)
            {
                if (charm.faction != this.faction) // Override hostile charm
                    charm.EndEffect();
                else if (charm.expirationTime < this.expirationTime) // Override same-faction charm that expires sooner
                    charm.EndEffect();
                else // A same-faction charm already lasts longer, this charm effect is unnecessary
                    Destroy(this);
            }
        }

        // Apply charm to target by changing its faction, and the faction of any attached spawners
        prevFaction = unit.faction;
        unit.faction = faction;
        unit.InitializeUnitFaction();

        foreach (Spawner spawner in unit.gameObject.GetComponents<Spawner>())
            spawner.faction = faction;
    }

    // What to do at the end of the effect
    protected override void OnEffectEnd()
    {
        // Remove charm from target by restoring its faction
        unit.faction = prevFaction;
        unit.InitializeUnitFaction();

        foreach (Spawner spawner in unit.gameObject.GetComponents<Spawner>())
            spawner.faction = prevFaction;
    }
}
