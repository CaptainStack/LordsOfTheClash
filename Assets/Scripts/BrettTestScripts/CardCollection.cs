using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    public Transform heroParent;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    void AddToDeck(Card card)
    {
        foreach (Transform child in heroParent)
        {
            if (child.GetComponent<Hero>().heroValue == card.heroValue)
            {
                child.GetComponent<Hero>().heroDeck.Add(card);
            }
        }
    }

}

