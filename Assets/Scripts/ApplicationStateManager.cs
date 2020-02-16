using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationStateManager : MonoBehaviour
{
    bool pauseMenuOn;
    GameObject pauseMenu;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = this.transform.Find("PauseMenu").gameObject;
        pauseMenuOn = false;

        player = this.transform.Find("Player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        if (pauseMenuOn) {
            pauseMenu.active = false;
            player.active = true;
            pauseMenuOn = false;
        } else {
            pauseMenu.active = true;
            player.active = false;
            pauseMenuOn = true;
        }
    }
}
