using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverSelect : MonoBehaviour
{
    public Button card; //in Unity editor set this as the button this script is attached to.
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectButton()
    {
        card.Select(); //sets 'resume button' to selected when you open the pause menu
        card.OnSelect(null);
    }
}
