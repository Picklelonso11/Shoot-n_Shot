using UnityEngine;
using UnityEngine.InputSystem;

public class MirillaMovement : MonoBehaviour
{
    private bool imPlayer1;
    private Vector2 direccion;
    public float speed = 200f;
    public RectTransform img;
    public RectTransform canvas;
    void Start()
    {
        //Código para averiguar quien tiene asignado el script y decidir que controles del teclado usa

        if (gameObject.CompareTag("Player1"))
        {
            imPlayer1 = true;
            // Es el jugador 1
        }
        else if (gameObject.CompareTag("Player2"))
        {
            imPlayer1 = false;
            // Es el jugador 2
        }
        else
        {
            Debug.Log("Error");
            // Se ha asignado a un objeto distinto del de los jugadores
        }
    }

    // Update is called once per frame
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
        float x = 0;
        float y = 0;

        if (Keyboard.current.aKey.isPressed)
        {
            //izquierda
            x -= 1;
        }
        if (Keyboard.current.wKey.isPressed)
        {
            //arriba
            y += 1;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            //abajo
            y -= 1;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            //derecha
            x += 1;
        }

        direccion = new Vector2(x, y).normalized;
        img.anchoredPosition += direccion * speed * Time.deltaTime;
        LimitarCanvas();
    }

    private void Player2Controls()
    {
        float x = 0;
        float y = 0;

        if (Keyboard.current.rightArrowKey.isPressed)
        {
            //derecha
            x += 1;
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            //izquierda
            x -= 1;
        }
        if (Keyboard.current.upArrowKey.isPressed)
        {
            //arriba
            y += 1;
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {
            //abajo
            y -= 1;
        }

        direccion = new Vector2(x, y).normalized;
        img.anchoredPosition += direccion * speed * Time.deltaTime;
        LimitarCanvas();
    }
    private void LimitarCanvas()
    {
        // Obtener la posición actual
        Vector2 pos = img.anchoredPosition;

        // Tamaño del canvas dividido entre 2 (para bordes)
        float halfW = canvas.rect.width / 2f;
        float halfH = canvas.rect.height / 2f;

        // Mitad del tamaño de la imagen
        float halfImgW = img.rect.width / 2f;
        float halfImgH = img.rect.height / 2f;

        // Limitar X dentro del canvas
        pos.x = Mathf.Clamp(
            pos.x,
            -halfW + halfImgW, // borde izquierdo
            halfW - halfImgW   // borde derecho
        );

        // Limitar Y dentro del canvas
        pos.y = Mathf.Clamp(
            pos.y,
            -halfH + halfImgH, // borde inferior
            halfH - halfImgH   // borde superior
        );

        // Aplicar posición corregida
        img.anchoredPosition = pos;
    }
}

