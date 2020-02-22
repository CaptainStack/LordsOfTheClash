using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTest : MonoBehaviour
{
    public int health;
    public Transform cardParent;
    public List<Card> heroDeck;

    void Start()
    {
        foreach (Transform child in cardParent)
        {
            heroDeck.Add(child.GetComponent<Card>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
