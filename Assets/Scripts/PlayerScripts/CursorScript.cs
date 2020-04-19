using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{
    //contains mouse input and creates mouse cursor.
    public Texture2D cursorSprite;
    public float horizontalSpeed = 2.0f; //Joystick Speed
    public float verticalSpeed = 2.0f;
    public float horizontalSpeedMouse = 2.0f; //Mouse Speed
    public float verticalSpeedMouse = 2.0f;
    public bool collidingWithButton;
    public GameObject collidingButton; //don't need to set object in editor. We get object from collider.

    int cursorWidth = 32;
    int cursorHeight = 32;
    public Vector2 cursorPosition;
    Vector2 targetPosition;//GUI cursor's target position
    Vector3 objectTargetPosition;//This object's target position (set to be same as cursor's)
    bool usingMouse;


    Vector2 screenBounds;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);//starts cursor at center of screen
    }

    private void OnGUI()
    {
        //Check if using mouse
        float h = Input.GetAxis("Mouse X"); 
        float v = Input.GetAxis("Mouse Y");
        if (h != 0 || v != 0)
        {
            usingMouse = true;
        }
        //check if using controller
        float controllerH = Input.GetAxis("Horizontal"); //joystick speed
        float controllerV = Input.GetAxis("Vertical");
        if (controllerH != 0 || controllerV != 0)
        {
            usingMouse = false;
        }

        //movespeed with joystick
        controllerH = horizontalSpeed * controllerH * Time.deltaTime;
        controllerV = verticalSpeed * controllerV * Time.deltaTime;

        //move cursor based on controller input and speed multiplier
        cursorPosition.x += controllerH;
        cursorPosition.y += controllerV;

        targetPosition = new Vector2(Mathf.Clamp(cursorPosition.x, 0, Screen.width-45), Mathf.Clamp(cursorPosition.y, 45, Screen.height-10)); //keep cursor on screen
        cursorPosition = targetPosition;
        objectTargetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);

        //If OS cursor is on screen, set the in-game cursor position to the OS cursor position
        if (IsCursorOnScreen())
        {
            if (usingMouse && !GetComponentInParent<ApplicationStateManager>().pauseMenuOn)
            {
                cursorPosition.x = Input.mousePosition.x;
                cursorPosition.y = Input.mousePosition.y;
            }
        }
      

        GUI.DrawTexture(new Rect(cursorPosition.x, Screen.height - cursorPosition.y, cursorWidth, cursorHeight), cursorSprite);
        this.transform.position = objectTargetPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision) //check if cursor is colliding with card button
    {
        if(collision.gameObject.tag == "Button")
        {
            collidingWithButton = true;
            collidingButton = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Button")
        {
            collidingWithButton = false;
        }
    }

    bool IsCursorOnScreen () //Check if OS cursor is on screen
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width)
        {
            return false;
        }
        if (Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            return false;
        }
        return true;
    }
}
