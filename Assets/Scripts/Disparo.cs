using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Disparo : MonoBehaviour
{
    private bool imPlayer1;
    public RectTransform mirilla; 
    public Camera camara;           

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
            DisparoJ1();
        }
        else
        {
            DisparoJ2();
        }
    }
    private void DisparoJ1()
    {
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            // Posicion de la mirilla en pantalla
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, mirilla.position);

            // Rayo desde esa posición
            Ray ray = camara.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Objetivo target = hit.collider.GetComponent<Objetivo>();

                // Si el objeto es una botella
                if (hit.collider.CompareTag("Botella"))
                {
                    target.Disparado(this);
                }
                else
                {
                    Debug.Log("Disparo fallido");
                }
            }
        }
    }
    private void DisparoJ2()
    {
        if (Keyboard.current.rightShiftKey.wasPressedThisFrame)
        {
            // Posicion de la mirilla en pantalla
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, mirilla.position);

            // Rayo desde esa posición
            Ray ray = camara.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Objetivo target = hit.collider.GetComponent<Objetivo>();

                // Si el objeto es una botella
                if (hit.collider.CompareTag("Botella"))
                {
                    target.Disparado(this);
                }
                else
                {
                    Debug.Log("Disparo fallido");
                }
            }
        }
    }
}
