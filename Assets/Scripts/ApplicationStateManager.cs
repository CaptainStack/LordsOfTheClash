using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationStateManager : MonoBehaviour
{
    bool pauseMenuOn;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenuOn = false;
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
            this.transform.Find("PauseMenu").gameObject.active = false;
            this.transform.Find("Player").gameObject.active = true;
            pauseMenuOn = false;
        } else {
            this.transform.Find("PauseMenu").gameObject.active = true;
            this.transform.Find("Player").gameObject.active = false;
            pauseMenuOn = true;
        }
    }
}
