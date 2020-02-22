using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card : MonoBehaviour
{
    public string cardName;
    public int manaCost; //how much it costs to summon
    public Effect effect;
    public int heroValue; //how to tell which hero the card came from


    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoCardAction()
    {
        effect.Action();
    }
}
