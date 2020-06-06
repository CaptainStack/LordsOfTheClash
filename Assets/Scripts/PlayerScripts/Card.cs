using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card : MonoBehaviour
{
    public string cardName;
    public int manaCost; //how much it costs to summon
    public int totalSummons = 1;
   // public Effect effect;
    public bool castAnywhere = true; //set to false if can only be cast on player's side.

    public Unit summon;

    public AreaOfEffect areaOfEffectPrefab;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoCardAction(Vector2 position, Faction faction, int totalSummons)
    {
        if (areaOfEffectPrefab != null) //Apply Area Of Effect at mouse position
        {
            AreaOfEffect newAreaOfEffect = Instantiate(areaOfEffectPrefab, position, Quaternion.identity);
            newAreaOfEffect.faction = faction;
            Mirror.NetworkServer.Spawn(newAreaOfEffect.gameObject);
        }
        if (summon != null) //summon unit at mouse position
        {
            for (int i = 0; i < totalSummons; i++)
            {
                Unit newUnit = Instantiate(summon, position, Quaternion.identity);
                newUnit.faction = faction;
                Mirror.NetworkServer.Spawn(newUnit.gameObject);
            }
        }
    }
}
