using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    int pauseFix;

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
        if (!pauseMenuOn && pauseFix  == 0) //makes it so player can't use abilities while the game is paused.
        {
            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1"))
            {
                player.GetComponent<Player>().UseCard(GetComponent<CursorScript>().cursorPosition);
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
            pauseFix = 0;
        }
    }
    private void SwitchSelectedCard()
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

    void TogglePauseMenu()
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
            Cursor.visible = true; //makes it so player can use mouse cursor to navigate menu.
            pauseFix = 1; //makes it so player doesn't use ability when turning off pause menu.
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
