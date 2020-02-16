using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    // Use this for initialization
    protected override void StartImpl ()
    {
        this.faction = Faction.Enemy;
	}

	// Update is called once per frame
    protected override void UpdateImpl()
    {
    }
}