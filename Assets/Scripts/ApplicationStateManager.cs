using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplicationStateManager : MonoBehaviour
{
    public bool pauseMenuOn;
    private GameObject pauseMenu;
    public Button resumeButton;//used to have the Resume Button in the pause menu start selected and highlighted.

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = this.transform.Find("PauseMenu").gameObject;
        pauseMenuOn = false;
        AudioManager.GetInstance().ChangeMusic("BattleMusic");

        // Target 60 fps
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseMenu();
        }

        ShowCursorWhilePaused();
    }

    public void TogglePauseMenu() //public so this can be accessed from a button
    {
        GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();

        if (pauseMenuOn)
        {
            pauseMenu.SetActive(false);
            pauseMenuOn = false;
            Cursor.visible = false;

            SetPauseState(false);
        } 
        else
        {
            pauseMenu.SetActive(true);
            pauseMenuOn = true;

            SetPauseState(true);

            if (resumeButton != null)
            {
                resumeButton.Select(); //sets 'resume button' to selected when you open the pause menu
                resumeButton.OnSelect(null);//sets resume button color to its selected color
            }
        }
    }

    public void SetPauseState(bool pause)
    {
        GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();

        if (pause)
        {
            // Pause game state logic if singleplayer
            if (gameStateManager && gameStateManager.isServer && Mirror.NetworkManager.singleton.numPlayers == 1)
                gameStateManager.Pause();

            foreach (Player player in Resources.FindObjectsOfTypeAll<Player>())
                player.Pause();
        }
        else
        {
            // Resume game state logic if singleplayer
            if (gameStateManager && gameStateManager.isServer && Mirror.NetworkManager.singleton.numPlayers == 1)
                gameStateManager.Resume();

            foreach (Player player in Resources.FindObjectsOfTypeAll<Player>())
                player.Resume();
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
}
