using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void SwitchScene(string newScene)
    {
        if (newScene != null)
        {
            if (newScene == "MainMenu")
            {
                AudioManager.GetInstance().ChangeMusic("TitleMusic");
            }
            Mirror.NetworkManager.singleton.ServerChangeScene(newScene);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
