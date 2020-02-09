using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            QuitGame();
        }
        if (Input.GetButton("Submit"))
        {
            SceneManager.LoadScene("Game");
        }
        if (Input.GetButton("Jump"))
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
