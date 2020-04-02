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
    public GameObject collidingButton;

    int cursorWidth = 32;
    int cursorHeight = 32;
    public Vector2 cursorPosition;
    Vector2 targetPosition; //used to move cursor position in GUI
    Vector3 objectTargetPosition; //used to move this object (object is a Vector3 which is why we need two targets)

    Vector2 screenBounds;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f); //start cursor at center of screen
    }

    private void OnGUI()
    {
        float h = horizontalSpeedMouse * Input.GetAxis("Mouse X") * Time.deltaTime; //mouse speed
        float v = verticalSpeedMouse * Input.GetAxis("Mouse Y") * Time.deltaTime;
        h += horizontalSpeed * Input.GetAxis("Horizontal") * Time.deltaTime; //mouse speed + joystick speed
        v += horizontalSpeed * Input.GetAxis("Vertical") * Time.deltaTime;

        cursorPosition.x += h;
        cursorPosition.y += v;

        targetPosition = new Vector2(Mathf.Clamp(cursorPosition.x, 0, Screen.width-45), Mathf.Clamp(cursorPosition.y, 45, Screen.height-10));
        cursorPosition = targetPosition;
        objectTargetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
        GUI.DrawTexture(new Rect(cursorPosition.x, Screen.height - cursorPosition.y, cursorWidth, cursorHeight), cursorSprite);
        this.transform.position = objectTargetPosition;//move this object to the position we moved the cursor
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
}
