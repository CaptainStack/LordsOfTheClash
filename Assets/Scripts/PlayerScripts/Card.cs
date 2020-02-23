using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card : MonoBehaviour
{
    public string cardName;
    public int manaCost; //how much it costs to summon
    public Effect effect;

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
