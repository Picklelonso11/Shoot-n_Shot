using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Disparo : MonoBehaviour
{
    [HideInInspector] public bool imPlayer1;
    public RectTransform mirilla;
    public Camera camara;
    private Gamepad myGamepad;

    void Start()
    {
        // Identificar jugador
        imPlayer1 = gameObject.CompareTag("Player1");

        // Asignar mando según tag
        if (Gamepad.all.Count >= 1 && imPlayer1)
        {
            myGamepad = Gamepad.all[0]; // Mando J1
        }
        if (Gamepad.all.Count >= 2 && !imPlayer1)
        {
            myGamepad = Gamepad.all[1]; // Mando J2
        }
    }
    void Update()
    {
        if (imPlayer1)
        {
            DisparoJugador(true);
        }
        else
        {
            DisparoJugador(false);
        }
    }

    private void DisparoJugador(bool player1)
    {
        bool disparo = false;

        // Entrada mando
        if (myGamepad != null && myGamepad.buttonSouth.wasPressedThisFrame)
        {
            disparo = true;
        }

        // Entrada teclado (backup)
        if (!disparo)
        {
            if (player1 && Keyboard.current.leftShiftKey.wasPressedThisFrame)
            {
                disparo = true;
            }
            if (!player1 && Keyboard.current.rightShiftKey.wasPressedThisFrame)
            {
                disparo = true;
            }
        }

        if (!disparo)
        {
            return;
        }
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, mirilla.position);
        Ray ray = camara.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Botella"))
            {
                Objetivo target = hit.collider.GetComponent<Objetivo>();
                target.Disparado(this);
            }
            else
            {
                Debug.Log("Disparo fallido");
            }
        }
    }
}
