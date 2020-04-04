using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplicationStateManager : MonoBehaviour
{
    private bool pauseMenuOn;
    private GameObject pauseMenu;
    private GameObject player;
    private SceneController sceneController;
    private Unit[] units;
    private float winLossTimer;
    public float eliminationTime;
    public int handSize = 2;
    bool pauseFix; //makes it so player doesn't use spell when pressing 'A' in the pause menu to resume game.
    public Button resumeButton;//used to have the Resume Button in the pause menu start selected and highlighted.
    public Button cardButton0;//used to highlight selected card
    public Button cardButton1;//used to highlight selected card

    // Start is called before the first frame update
    void Start()
    {
        sceneController = GetComponent<SceneController>();
        pauseMenu = this.transform.Find("PauseMenu").gameObject;
        pauseMenuOn = false;
        player = this.transform.Find("Player").gameObject;
        Time.timeScale = 1.0f;
        AudioManager.GetInstance().ChangeMusic("BattleMusic");

        // Target 60 fps
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        winLossTimer -= Time.deltaTime;
        var winner = Faction.Neutral;

        if (winLossTimer <= 0f)
        {
            winLossTimer = eliminationTime;
            winner = WinningFaction();
        }

        if (winner == Faction.Friendly)
        {
            sceneController.SwitchScene("MainMenu");
        }
        else if (winner == Faction.Enemy)
        {
            sceneController.SwitchScene("GameOver");
        }

        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseMenu();
        }

        if (!pauseMenuOn && !pauseFix) //makes it pressing "Fire1" to unpause doesn't also make you use a spell.
        {
            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1"))
            {
                if (GetComponentInChildren<CursorScript>().collidingWithButton)
                {
                    player.GetComponent<Player>().cardSelected = GetComponentInChildren<CursorScript>().collidingButton.GetComponent<ButtonNumber>().cardNumber;
                }
                else
                {
                    player.GetComponent<Player>().UseCard(GetComponentInChildren<CursorScript>().cursorPosition);
                }
            }

            if (Input.GetButtonDown("Card1"))
            {
                player.GetComponent<Player>().cardSelected = 0;
            }

            if (Input.GetButtonDown("Card2"))
            {
                player.GetComponent<Player>().cardSelected = 1;
            }
            
            SwitchSelectedCard();
        }

        if (!pauseMenuOn)
        {
            pauseFix = false; //now pressing "Fire1" will use a spell again
        }

        ShowCursorWhilePaused();
        HighlightSelectedCard();
    }

    private void SwitchSelectedCard() //use bumpers to change selected card
    {
        if (Input.GetButtonDown("NextCard"))
        {
            if (player.GetComponent<Player>().cardSelected == handSize - 1)
            {
                player.GetComponent<Player>().cardSelected = 0;
            }
            else
            {
                player.GetComponent<Player>().cardSelected += 1;
            }
        }
        if (Input.GetButtonDown("PreviousCard"))
        {
            if (player.GetComponent<Player>().cardSelected != 0)
            {
                player.GetComponent<Player>().cardSelected -= 1;
            }
            else
            {
                player.GetComponent<Player>().cardSelected = handSize - 1;
            }
        }
    }

    public void TogglePauseMenu() //public so this can be accessed from a button
    {
        if (pauseMenuOn)
        {
            pauseMenu.SetActive(false);
            player.SetActive(true);
            pauseMenuOn = false;
            Time.timeScale = 1.0f;
            Cursor.visible = false;
        } 
        else
        {
            pauseMenu.SetActive(true);
            player.SetActive(false);
            pauseMenuOn = true;
            Time.timeScale = 0.0f;
            pauseFix = true; //makes it so player doesn't use ability when turning off pause menu.
            if (resumeButton != null)
            {
                resumeButton.Select(); //sets 'resume button' to selected when you open the pause menu
                resumeButton.OnSelect(null);//sets resume button color to its selected color
            }
        }
    }

    void HighlightSelectedCard() //Highlights card currently selected
    {
        if (player.GetComponent<Player>().cardSelected == 0 && !pauseMenuOn)
        {
            cardButton0.Select(); //Selects button
            cardButton0.OnSelect(null); //highlights button
        }
        else if (player.GetComponent<Player>().cardSelected == 1 && !pauseMenuOn)
        {
            cardButton1.Select();
            cardButton1.OnSelect(null);
        }
    }

    void ShowCursorWhilePaused()
    {
        if (pauseMenuOn && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            Cursor.visible = true;//Make OS cursor appear while paused if you get mouse input
        }
        else if (pauseMenuOn && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            Cursor.visible = false; //OS cursor disappear in pause menu if controller used
        }
    }

    Faction WinningFaction()
    {
        units = FindObjectsOfType<Unit>();
        int enemies = 0;
        int friends = 0;

        foreach (Unit unit in units)
        {
            if (unit.faction == Faction.Enemy)
            {
                enemies++;
            }
            else if (unit.faction == Faction.Friendly)
            {
                friends++;
            }
        }

        if (enemies == 0)
        {
            return Faction.Friendly;
        } 
        else if (friends == 0)
        {
            return Faction.Enemy;
        } 
        else
        {
            return Faction.Neutral;
        }
    }
}
