﻿using System.Collections;
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
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void SwitchScene(string newScene)
    {
        if (newScene != null)
        {
            SceneManager.LoadScene(newScene);
        }
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}