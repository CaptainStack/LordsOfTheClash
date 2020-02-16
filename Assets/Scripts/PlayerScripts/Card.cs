using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card : MonoBehaviour
{

    public int manaCost; //how much it costs to summon

    [Space]
    [Header("Card Object:")] //drag card object in from Unity
    public Effect effect;


    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CardAction() //call to summon a creature or use a spell.
    {
        effect.Action();
    }

}
