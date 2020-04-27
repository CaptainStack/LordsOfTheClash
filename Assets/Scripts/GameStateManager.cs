using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStateManager : Mirror.NetworkBehaviour
{
    private float winLossTimer;
    public float eliminationTime;
    private SceneController sceneController;

    // Start is called before the first frame update
    void Start()
    {
        sceneController = GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
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
        }
    }
    Faction WinningFaction()
    {
        int enemies = 0;
        int friends = 0;

        foreach (Unit unit in FindObjectsOfType<Unit>())
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

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }
}
