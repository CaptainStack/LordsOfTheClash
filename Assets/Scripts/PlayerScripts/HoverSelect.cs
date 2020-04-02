using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverSelect : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectButton()//Called in an EventTrigger on menu buttons
    {
        button.Select(); //sets card object when you hover over it
        button.OnSelect(null);//highlights card
    }
}
