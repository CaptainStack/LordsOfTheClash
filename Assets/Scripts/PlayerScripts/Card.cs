using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card : MonoBehaviour
{
    public string cardName;
    public int manaCost; //how much it costs to summon
    public int totalSummons = 1;
    public Effect effect;

    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoCardAction(Vector2 position, Faction faction, int totalSummons)
    {
        effect.Action(position, faction, totalSummons);
    }
}
