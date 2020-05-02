using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System.Linq;

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

    bool isWindows;
    bool isMac;
    
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

    List<string> controllerList;
    string[] stringArray;

    void Start()
    {
        stringArray = Input.GetJoystickNames();
        controllerList = stringArray.ToList();
        foreach (string str in controllerList)
        {
            Debug.Log(str);
        }
        playerCanvas = (Canvas)gameObject.GetComponentInChildren(typeof(Canvas), true);

        networkIdentity = gameObject.GetComponent<Mirror.NetworkIdentity>();
        if (!networkIdentity)
            networkIdentity = gameObject.AddComponent<Mirror.NetworkIdentity>();

        playerCursor = GetComponentInChildren<CursorScript>();

        FillDeck();
        FillHand();

        if (!isLocalPlayer)
        {
            // Disable all non-local player canvas UI elements
            playerCanvas.gameObject.SetActive(false);
            playerCursor.gameObject.SetActive(false);
        }

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            isWindows = true;
        }
        else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            isMac = true;
        }

    }

    void Update()
    {
        if (!isLocalPlayer || isPaused)
            return;

        manaText.text = "Mana: " + currentMana.ToString();
        ManaRegen();

        handText.text = $"Hand: [";
        foreach(Card card in playerHand)
        {
            handText.text += $"{card.cardName} ({card.manaCost}), ";
        }
        handText.text += "]";

        deckText.text = $"Deck: [";
        foreach(Card card in playerDeck)
        {
            deckText.text += $"{card.cardName} ({card.manaCost}), ";
        }
        deckText.text += "]";

        cardButton1Text.text = playerHand[0].cardName;
        cardButton2Text.text = playerHand[1].cardName;

        if (isWindows)
        {
            HandleInputWindows();
        }
        else if (isMac)
        {
            HandleInputMac();
        }

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

    void HandleInputWindows()
    {
        if (Input.GetButtonDown("Fire1Windows"))
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
        
        SwitchSelectedCardWindows();
    }

    void HandleInputMac()
    {
        if (Input.GetButtonDown("Fire1Mac"))
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

        SwitchSelectedCardMac();
    }

    private void SwitchSelectedCardWindows() //use bumpers to change selected card
    {
        if (Input.GetButtonDown("NextCardWindows"))
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
        if (Input.GetButtonDown("PreviousCardWindows"))
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

    private void SwitchSelectedCardMac()
    {
        if (Input.GetButtonDown("NextCardMac"))
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
        if (Input.GetButtonDown("PreviousCardMac"))
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
            currentMana -= playerHand[cardSelected].manaCost;

            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, Camera.main.nearClipPlane));
            point.z = 0f;

            Faction faction = isServer ? Faction.Friendly : Faction.Enemy;

            // Tell the server to do the card action
            CmdDoCardAction(cardSelected, (Vector2)point, faction);

            // Update local player deck
            RemoveCardFromHand();
            DrawCardFromDeck();
        }
    }

    // Send a command from the client to the server, asking it to use the selected card at the given target position
    [Mirror.Command]
    void CmdDoCardAction(int cardIndexInHand, Vector2 position, Faction faction)
    {
        cardSelected = cardIndexInHand;
        playerHand[cardSelected].DoCardAction(position, faction);

        // Update server's card deck and hand so it matches client
        if (!isLocalPlayer) // Skip if server == local player, because deck was already updated for local player before sending the command
        {
            RemoveCardFromHand();
            DrawCardFromDeck();
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

}
