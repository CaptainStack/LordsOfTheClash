using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes it so cursor will appear in main menu after a victory 
//Makes it so cursor can disappear in main menu if controller is being used.
public class MainMenuInput : MonoBehaviour
{
    bool usingCursor;

    void Start()
    {
        Cursor.visible = false;
        usingCursor = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        float h = Input.GetAxis("Mouse X"); //mouse input
        float v = Input.GetAxis("Mouse Y");
        float controllerH = Input.GetAxis("Horizontal"); //Joystick input
        float controllerV = Input.GetAxis("Vertical"); 

        if (controllerH != 0 || controllerV != 0 && usingCursor) //if the mouse isn't moving for 5 seconds set 'usingCursor' to false
        {
                usingCursor = false;
                Cursor.visible = false;   
        }

        else if (h != 0 || v != 0 && !usingCursor) //if cursor moved set usingCursor to true
        {
            usingCursor = true;
            Cursor.visible = true;
        }
    }
}
