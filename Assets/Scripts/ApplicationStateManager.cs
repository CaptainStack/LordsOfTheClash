using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplicationStateManager : MonoBehaviour
{
    public bool pauseMenuOn;
    private GameObject pauseMenu;
    private Player player;
    public Button resumeButton;//used to have the Resume Button in the pause menu start selected and highlighted.

    private GameStateManager gameStateManager;

    // Start is called before the first frame update
    void Start()
    {
        gameStateManager = FindObjectOfType<GameStateManager>();
        pauseMenu = this.transform.Find("PauseMenu").gameObject;
        pauseMenuOn = false;
        player = FindObjectOfType<Player>();
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
        gameStateManager = FindObjectOfType<GameStateManager>();

        if (pauseMenuOn)
        {
            pauseMenu.SetActive(false);
            pauseMenuOn = false;
            Cursor.visible = false;

            if (gameStateManager)
                gameStateManager.Resume();

            if (player)
                player.gameObject.SetActive(true);
        } 
        else
        {
            pauseMenu.SetActive(true);
            pauseMenuOn = true;

            if (gameStateManager)
                gameStateManager.Pause();

            if (player)
                player.gameObject.SetActive(false);

            if (resumeButton != null)
            {
                resumeButton.Select(); //sets 'resume button' to selected when you open the pause menu
                resumeButton.OnSelect(null);//sets resume button color to its selected color
            }
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
