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

    Vector2 screenBounds;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);//starts cursor at center of screen
    }

    private void OnGUI()
    {
       
        float h = horizontalSpeedMouse * Input.GetAxis("Mouse X") * Time.deltaTime; //mouse speed
        float v = verticalSpeedMouse * Input.GetAxis("Mouse Y") * Time.deltaTime;
        float controllerH = Input.GetAxis("Horizontal"); //joystick speed
        float controllerV = Input.GetAxis("Vertical");

        h += horizontalSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;  //final x speed is mouse speed + controller speed
        v += horizontalSpeed * Input.GetAxis("Vertical") * Time.deltaTime;

        cursorPosition.x += h;
        cursorPosition.y += v;

        targetPosition = new Vector2(Mathf.Clamp(cursorPosition.x, 0, Screen.width-45), Mathf.Clamp(cursorPosition.y, 45, Screen.height-10)); //keep cursor on screen
        cursorPosition = targetPosition;
        objectTargetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
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
}
