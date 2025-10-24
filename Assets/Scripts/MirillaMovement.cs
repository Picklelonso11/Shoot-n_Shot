using UnityEngine;
using UnityEngine.InputSystem;

public class MirillaMovement : MonoBehaviour
{
    private bool imPlayer1;
    private Vector2 direccion;
    void Start()
    {
        //Código para averiguar quien tiene asignado el script y decidir que controles del teclado usa

        if (gameObject.CompareTag("Player1"))
        {
            imPlayer1 = true;
            // Es el jugador 1
        }
        else if (gameObject.CompareTag("Enemigo"))
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

    }
    private void Player1Controls()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {


        }
        if (Input.GetKeyDown(KeyCode.W))
        {


        }
        if (Input.GetKeyDown(KeyCode.S))
        {


        }
        if (Input.GetKeyDown(KeyCode.D))
        {


        }
    }
    private void Player2Controls()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {


        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {


        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {


        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {


        }
    }
}
