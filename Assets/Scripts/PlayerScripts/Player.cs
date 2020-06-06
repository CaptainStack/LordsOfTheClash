using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : Mirror.NetworkBehaviour
{
    public float totalMana;
    public float manaRechargeTime;
    public int playerHandSize;

    public Text manaText;
    public Text handText;
    public Text deckText;
    public Button cardButton1;
    public Button cardButton2;
    public Text cardButton1Text;
    public Text cardButton2Text;
    public CursorScript playerCursor;
    public GameObject player1Side;//cannot be null (don't need to pass through unity though, the code grabs it)
    public GameObject player2Side;//cannot be null (don't need to pass through unity though, the code grabs it)
    public GameObject[] neutralColliders;

    [Mirror.SyncVar]
    public float currentMana;
    public int cardSelected; //player input on button click determines which card is selected
    float timer;

    // Used for networking
    private Mirror.NetworkIdentity networkIdentity;

    public List<Hero> heroes = new List<Hero>();
    public List<Card> playerDeck = new List<Card>();
    public List<Card> playerHand = new List<Card>(); //Cards in player's hand

    private bool isPaused = false;
    private Canvas playerCanvas;

    void Start()
    {
        playerCanvas = (Canvas)gameObject.GetComponentInChildren(typeof(Canvas), true);

        networkIdentity = gameObject.GetComponent<Mirror.NetworkIdentity>();
        if (!networkIdentity)
            networkIdentity = gameObject.AddComponent<Mirror.NetworkIdentity>();

        playerCursor = GetComponentInChildren<CursorScript>();

        player1Side = GameObject.FindGameObjectWithTag("Player1Side");
        player2Side = GameObject.FindGameObjectWithTag("Player2Side");
        neutralColliders = GameObject.FindGameObjectsWithTag("NeutralSide"); //put all neutral colliders in the scene in an array

        for (int i = 0; i < neutralColliders.Length; i++) //set the neutral colliders to inactive (they get set active in Building script when building is destroyed)
        {
            neutralColliders[i].SetActive(false);
        }

        FillDeck();
        FillHand();

        if (!isLocalPlayer)
        {
            // Disable all non-local player canvas UI elements
            playerCanvas.gameObject.SetActive(false);
            playerCursor.gameObject.SetActive(false);
        }

        Faction faction = isServer ? Faction.Friendly : Faction.Enemy;
        if (faction == Faction.Friendly && player1Side != null) //hide the sprite showing where you can't summon depending on which faction you are.
        {
            player1Side.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        else if (faction == Faction.Enemy && player2Side != null)//if Start() only runs for Player1 then this condition will never evaluate to true
        {
            player2Side.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        ShowUsableArea();
    }

    void Update()
    {
        if (!isLocalPlayer || isPaused)
            return;

        manaText.text = "Mana: " + currentMana.ToString();
        ManaRegen();

        handText.text = $"Hand: [";
        foreach (Card card in playerHand)
        {
            handText.text += $"{card.cardName} ({card.manaCost}), ";
        }
        handText.text += "]";

        deckText.text = $"Deck: [";
        foreach (Card card in playerDeck)
        {
            deckText.text += $"{card.cardName} ({card.manaCost}), ";
        }
        deckText.text += "]";

        cardButton1Text.text = playerHand[0].cardName;
        cardButton2Text.text = playerHand[1].cardName;

        HandleInput();
        // Highlight selected card, if canvas is active
        if (playerCanvas.gameObject.activeInHierarchy)
            HighlightSelectedCard();
    }

    public void Pause()
    {
        PauseInternal(true);
    }

    public void Resume()
    {
        PauseInternal(false);
    }

    // Internal Player pause implementation
    private void PauseInternal(bool pauseState)
    {
        // Set pause state if singleplayer
        if (isServer && Mirror.NetworkManager.singleton.numPlayers == 1)
            isPaused = pauseState;

        // Toggle canvas buttons / UI visibility while paused, even in multiplayer, but only for the local player
        if (isLocalPlayer)
        {
            playerCanvas.gameObject.SetActive(!pauseState);
            playerCursor.gameObject.SetActive(!pauseState);
        }
    }

    void HandleInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (playerCursor.collidingWithButton)
            {
                cardSelected = playerCursor.collidingButton.GetComponent<ButtonNumber>().cardNumber;
            }
            else
            {
                UseCard(playerCursor.cursorPosition);
            }
        }

        if (Input.GetButtonDown("Card1"))
        {
            cardSelected = 0;
        }

        if (Input.GetButtonDown("Card2"))
        {
            cardSelected = 1;
        }

        SwitchSelectedCard();
    }

    private void SwitchSelectedCard() //use bumpers to change selected card
    {
        if (Input.GetButtonDown("NextCard"))
        {
            if (cardSelected == playerHandSize - 1)
            {
                cardSelected = 0;
            }
            else
            {
                cardSelected += 1;
            }
        }
        if (Input.GetButtonDown("PreviousCard"))
        {
            if (cardSelected != 0)
            {
                cardSelected -= 1;
            }
            else
            {
                cardSelected = playerHandSize - 1;
            }
        }
    }

    void HighlightSelectedCard() //Highlights card currently selected
    {
        if (cardSelected == 0)
        {
            cardButton1.Select(); //Selects button
            cardButton1.OnSelect(null); //highlights button          
        }
        else if (cardSelected == 1)
        {
            cardButton2.Select();
            cardButton2.OnSelect(null);
        }
        ShowUsableArea();
    }

    void ManaRegen()
    {
        if (currentMana < totalMana)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                currentMana++;
                timer = manaRechargeTime;
            }
        }
    }

    //press button on keyboard to use card 
    //Make public to call from UI?
    public void UseCard(Vector2 position)
    {
        // Ignore clicks over UI elements and check mana
        if (!EventSystem.current.IsPointerOverGameObject() && !playerCursor.GetComponent<CursorScript>().collidingWithButton && currentMana >= playerHand[cardSelected].manaCost)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, Camera.main.nearClipPlane));
            point.z = 0f;
            Faction faction = isServer ? Faction.Friendly : Faction.Enemy;

            if (playerHand[cardSelected].castAnywhere == true) //check if spell can be cast anywhere on the map
            {
                currentMana -= playerHand[cardSelected].manaCost;

                // Tell the server to do the card action
                CmdDoCardAction(cardSelected, (Vector2)point, faction, playerHand[cardSelected].totalSummons);

                // Update local player deck
                RemoveCardFromHand();
                DrawCardFromDeck();
            }
            else if ((faction == Faction.Friendly && player1Side.GetComponent<BoxCollider2D>().bounds.Contains((Vector2)point)) || (faction == Faction.Enemy && player2Side.GetComponent<BoxCollider2D>().bounds.Contains((Vector2)point)))
            {
                currentMana -= playerHand[cardSelected].manaCost;

                // Tell the server to do the card action
                CmdDoCardAction(cardSelected, (Vector2)point, faction, playerHand[cardSelected].totalSummons);

                // Update local player deck
                RemoveCardFromHand();
                DrawCardFromDeck();
            }
            else if (CollidingWithNeutralSide(point, neutralColliders))
            {
                currentMana -= playerHand[cardSelected].manaCost;

                // Tell the server to do the card action
                CmdDoCardAction(cardSelected, (Vector2)point, faction, playerHand[cardSelected].totalSummons);

                // Update local player deck
                RemoveCardFromHand();
                DrawCardFromDeck();
            }
        }
    }

    // Send a command from the client to the server, asking it to use the selected card at the given target position
    [Mirror.Command]
    void CmdDoCardAction(int cardIndexInHand, Vector2 position, Faction faction, int totalSummons)
    {
        bool sideNeutral = CollidingWithNeutralSide(position, neutralColliders);
        if (playerHand[cardSelected].castAnywhere == true)
        {
            cardSelected = cardIndexInHand;
            playerHand[cardSelected].DoCardAction(position, faction, playerHand[cardSelected].totalSummons);

            // Update server's card deck and hand so it matches client
            if (!isLocalPlayer) // Skip if server == local player, because deck was already updated for local player before sending the command
            {
                RemoveCardFromHand();
                DrawCardFromDeck();
            }
        }
        else if ((faction == Faction.Friendly && player1Side.GetComponent<BoxCollider2D>().bounds.Contains((Vector2)position)) || (faction == Faction.Enemy && player2Side.GetComponent<BoxCollider2D>().bounds.Contains((Vector2)position)))
        {
            cardSelected = cardIndexInHand;
            playerHand[cardSelected].DoCardAction(position, faction, playerHand[cardSelected].totalSummons);

            // Update server's card deck and hand so it matches client
            if (!isLocalPlayer) // Skip if server == local player, because deck was already updated for local player before sending the command
            {
                RemoveCardFromHand();
                DrawCardFromDeck();
            }
        }
        else if (sideNeutral)
        {
            cardSelected = cardIndexInHand;
            playerHand[cardSelected].DoCardAction(position, faction, playerHand[cardSelected].totalSummons);

            // Update server's card deck and hand so it matches client
            if (!isLocalPlayer) // Skip if server == local player, because deck was already updated for local player before sending the command
            {
                RemoveCardFromHand();
                DrawCardFromDeck();
            }
        }
    }

    void RemoveCardFromHand()
    {
        playerDeck.Add(playerHand[cardSelected]);
        playerHand.RemoveAt(cardSelected);
    }
    void DrawCardFromDeck()
    {
        playerHand.Add(playerDeck[cardSelected]);
        playerDeck.RemoveAt(cardSelected);
    }

    void FillDeck()
    {
        foreach (Hero hero in heroes)
        {
            foreach (Card card in hero.heroDeck)
            {
                playerDeck.Add(card);
            }
        }
    }

    void FillHand()
    {
        while (playerHand.Count < playerHandSize)
        {
            DrawCardFromDeck();
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            Card temp = playerDeck[i];
            int placementIndex = Random.Range(0, playerDeck.Count - 1);
            playerDeck[i] = playerDeck[placementIndex];
            playerDeck[placementIndex] = temp;
        }
    }

    public void SelectCard0() //on button click select card 0
    {
        cardSelected = 0;
        Debug.Log(playerHand[cardSelected].name);
    }

    public void SelectCard1() //on button click select card 1
    {
        cardSelected = 1;
        Debug.Log(playerHand[cardSelected].name);
    }

    public bool CollidingWithNeutralSide(Vector2 cursorPosition, GameObject[] neutralColliders) //return true if colliding w/ neutral, otherwise return false
    {
        int size = neutralColliders.Length;
        for (int i = 0; i < size; i++)
        {
            if (neutralColliders[i].GetComponent<BoxCollider2D>().bounds.Contains((Vector2)cursorPosition))
            {
                return true;
            }
        }
        return false;
    }

    void ShowUsableArea()//Highlight area in red if the player can't use the card there. (Called in Start() and in HighlightSelectedCard())
    {
        Faction faction = isServer ? Faction.Friendly : Faction.Enemy;
        if (playerHand[cardSelected].castAnywhere)
        {
            if (faction == Faction.Friendly && player2Side != null)
            {
                player2Side.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            else if (faction == Faction.Enemy && player1Side != null)
            {
                player1Side.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }

        }
        else
        {
            if (faction == Faction.Friendly && player2Side != null)
            {
                player2Side.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
            else if (faction == Faction.Enemy && player1Side != null)
            {
                player1Side.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
        }

    }
}
