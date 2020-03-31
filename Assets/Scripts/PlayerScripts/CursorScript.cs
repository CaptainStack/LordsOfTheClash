using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public Texture2D cursorSprite;
    public float horizontalSpeed = 2.0f;
    public float verticalSpeed = 2.0f;

    int cursorWidth = 32;
    int cursorHeight = 32;
    public Vector2 cursorPosition;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void OnGUI()
    {
        float h = horizontalSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        float v = verticalSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
        h += horizontalSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        v += horizontalSpeed * Input.GetAxis("Vertical") * Time.deltaTime;

        cursorPosition.x += h;
        cursorPosition.y += v;

        GUI.DrawTexture(new Rect(cursorPosition.x, Screen.height - cursorPosition.y, cursorWidth, cursorHeight), cursorSprite);
    }
}
