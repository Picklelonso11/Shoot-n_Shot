using UnityEngine;

public class SpawnPuerta : MonoBehaviour
{
    // Referencia a la puerta asociada a este spawn
    public Transform puerta;

    // Indica si pertenece al lado izquierdo
    public bool esIzquierda;

    public AudioSource sonidoPuerta;

    private Quaternion rotacionInicial; // Rotación original de la puerta

    void Awake()
    {
        // Guardamos la rotación inicial para poder cerrarla después
        if (puerta != null)
        {
            rotacionInicial = puerta.rotation;
        }
    }

    // Se llama cuando spawnea una botella en este punto
    public void AbrirPuerta()
    {
        if (puerta == null) return;

        // Decide el ángulo según si es izquierda o derecha
        float angulo = esIzquierda ? 100f : -100f;

        // Aplica la rotación
        puerta.rotation = rotacionInicial;
        puerta.Rotate(0f, angulo, 0f);
    }

    // Se llama cuando la botella es destruida
    public void CerrarPuerta()
    {
        if (puerta == null) return;

        sonidoPuerta.Play();

        // Vuelve exactamente a la rotación original
        puerta.rotation = rotacionInicial;
    }
}
