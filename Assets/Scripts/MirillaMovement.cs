using UnityEngine;
using UnityEngine.InputSystem;

public class MirillaMovement : MonoBehaviour
{
    private bool imPlayer1;

    public float speed = 200f;
    public RectTransform img;
    public RectTransform canvas;

    private Gamepad myGamepad;

    void Start()
    {
        // Decide si este objeto es Player1 o Player2
        imPlayer1 = gameObject.CompareTag("Player1");

        // Asigna el mando correspondiente
        if (Gamepad.all.Count >= 1)
        {
            if (imPlayer1)
            {
                myGamepad = Gamepad.all[0]; // Primer mando
            }
            else if (Gamepad.all.Count >= 2)
            {
                myGamepad = Gamepad.all[1]; // Segundo mando
            }

        }
    }


    void Update()
    {
        if (imPlayer1)
        {
            Player1Controls();
        }
        else
        {
            Player2Controls();
        }
    }

    private void Player1Controls()
    {
        // Leer del mando si existe
        Vector2 stick = myGamepad != null ? myGamepad.leftStick.ReadValue() : Vector2.zero;

        // Si el mando no se mueve, permitir teclado también
        if (stick == Vector2.zero)
        {
            float x = 0;
            float y = 0;

            if (Keyboard.current.aKey.isPressed) x -= 1;
            if (Keyboard.current.dKey.isPressed) x += 1;
            if (Keyboard.current.wKey.isPressed) y += 1;
            if (Keyboard.current.sKey.isPressed) y -= 1;

            stick = new Vector2(x, y).normalized;
        }

        img.anchoredPosition += stick * speed * Time.deltaTime;
        LimitarCanvas();
    }

    private void Player2Controls()
    {
        Vector2 stick = myGamepad != null ? myGamepad.leftStick.ReadValue() : Vector2.zero;
        if (stick == Vector2.zero)
        {
            float x = 0;
            float y = 0;

            if (Keyboard.current.leftArrowKey.isPressed) x -= 1;
            if (Keyboard.current.rightArrowKey.isPressed) x += 1;
            if (Keyboard.current.upArrowKey.isPressed) y += 1;
            if (Keyboard.current.downArrowKey.isPressed) y -= 1;

            stick = new Vector2(x, y).normalized;
        }
        img.anchoredPosition += stick * speed * Time.deltaTime;
        LimitarCanvas();
    }
    private void LimitarCanvas()
    {

        Vector2 pos = img.anchoredPosition;
        

        float halfW = canvas.rect.width / 2f;
        float halfH = canvas.rect.height / 2f;
       

        float halfImgW = img.rect.width / 2f;
        float halfImgH = img.rect.height / 2f;

        pos.x = Mathf.Clamp(pos.x, -halfW + halfImgW, halfW - halfImgW);
        pos.y = Mathf.Clamp(pos.y, -halfH + halfImgH, halfH - halfImgH);
        img.anchoredPosition = pos;
    }
}

