using UnityEngine;
using UnityEngine.InputSystem;

public class MirillaMovement : MonoBehaviour
{
    private bool imPlayer1;

    [Header("Velocidad")]
    public float velocidadMax = 1100f;   // Velocidad máxima que puede alcanzar la mirilla
    public float suavizado = 0.1f;     // Tiempo de aceleración y frenado (más alto = más suave)

    public RectTransform img;
    public RectTransform canvas;

    private Gamepad myGamepad;

    // Velocidad real de la mirilla en este momento
    private Vector2 velocidadActual;

    // Variable interna necesaria para SmoothDamp
    private Vector2 velocidadRef;

    void Start()
    {
        // Decide si este objeto es Player1 o Player2
        imPlayer1 = gameObject.CompareTag("Player1");

        // Asigna el mando correspondiente
        if (Gamepad.all.Count >= 1)
        {
            if (imPlayer1)
            {
                myGamepad = Gamepad.all[0];   // Primer mando
            }
            else if (Gamepad.all.Count >= 2)
            {
                myGamepad = Gamepad.all[1];   // Segundo mando
            }
        }
    }

    void Update()
    {
        // Obtener dirección de entrada (mando o teclado)
        Vector2 input = ObtenerInput();

        // Convertir dirección en velocidad objetivo
        Vector2 velocidadObjetivo = input * velocidadMax;

        // Suavizar aceleración y desaceleración
        velocidadActual = Vector2.SmoothDamp(velocidadActual, velocidadObjetivo, ref velocidadRef, suavizado);

        // Aplicar movimiento usando la velocidad suavizada
        img.anchoredPosition += velocidadActual * Time.deltaTime;

        // Evitar que salga del canvas
        LimitarCanvas();
    }

    // Obtiene el input combinando mando y teclado
    private Vector2 ObtenerInput()
    {
        // Leer stick del mando si existe
        Vector2 stick = myGamepad != null ? myGamepad.leftStick.ReadValue() : Vector2.zero;

        // Si el stick está quieto, permitir teclado
        if (stick == Vector2.zero)
        {
            float x = 0;
            float y = 0;

            if (imPlayer1)
            {
                if (Keyboard.current.aKey.isPressed) x -= 1;
                if (Keyboard.current.dKey.isPressed) x += 1;
                if (Keyboard.current.wKey.isPressed) y += 1;
                if (Keyboard.current.sKey.isPressed) y -= 1;
            }
            else
            {
                if (Keyboard.current.leftArrowKey.isPressed) x -= 1;
                if (Keyboard.current.rightArrowKey.isPressed) x += 1;
                if (Keyboard.current.upArrowKey.isPressed) y += 1;
                if (Keyboard.current.downArrowKey.isPressed) y -= 1;
            }

            stick = new Vector2(x, y).normalized;
        }

        return stick;
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

    void Borrachera(int chupitosBebidos)
    {
        switch (chupitosBebidos)
        {
            case 0:
                suavizado = 0.1f;
                break;

            case 1:
                suavizado = 0.125f;
                break;

            case 2:
                suavizado = 0.15f;
                break;

            case 3:
                suavizado = 0.175f;
                break;

            case 4:
                suavizado = 0.2f;
                break;
        }
    }
}

