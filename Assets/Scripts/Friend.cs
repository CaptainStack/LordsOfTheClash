using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : Character
{
    // Use this for initialization
    protected override void StartImpl ()
    {
        this.faction = Faction.Friendly;
	}

	// Update is called once per frame
    protected override void UpdateImpl()
    {
    }
}

