using UnityEngine;

// A special type of melee unit that only targets buildings
public class AntiBuildingMeleeUnit : MeleeUnit
{
    override protected bool OnlyTargetBuildings()
    {
        return true;
    }
}