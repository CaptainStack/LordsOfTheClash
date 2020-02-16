using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMenuScript : MonoBehaviour
    //Choose card from menu and add that to deck
    //Make deck Object
    //want to attach cards to each hero, then fill the player deck.
    //
{
    public Hero hero1;
    public Hero hero2;
    public Hero hero3;
    public Hero hero4;
    List<Hero> heroes = new List<Hero>();
    void Start()
    {
        heroes.Add(hero1);
        heroes.Add(hero2);
        heroes.Add(hero3);
        heroes.Add(hero4);
    }

    void Update()
    {
        
    }

    void FillDecks()
    {
        for (int i = 0; i < 3; i++) //loop through heroes
        {
            heroes[i].FillCards();
        }
    }

}
