using UnityEngine;
using System.Collections;


public class SpawnPuerta : MonoBehaviour
{
    // Referencia a la puerta asociada a este spawn
    public Transform puerta;

    // Indica si pertenece al lado izquierdo
    public bool esIzquierda;

    public AudioSource sonidoPuerta;

    public float velocidadRotacion = 2f; // Velocidad de apertura/cierre

    private Quaternion rotacionInicial; // Rotación original de la puerta
    private Coroutine coroutineActual;

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

        float angulo = esIzquierda ? 100f : -100f;
        Quaternion rotacionObjetivo = rotacionInicial * Quaternion.Euler(0f, angulo, 0f);

        if (coroutineActual != null) StopCoroutine(coroutineActual);
        coroutineActual = StartCoroutine(RotarPuerta(rotacionObjetivo));
    }

    // Se llama cuando la botella es destruida
    public void CerrarPuerta()
    {
        if (puerta == null) return;

        sonidoPuerta.Play();

        if (coroutineActual != null) StopCoroutine(coroutineActual);
        coroutineActual = StartCoroutine(RotarPuerta(rotacionInicial));
    }
    private IEnumerator RotarPuerta(Quaternion destino)
    {
        while (Quaternion.Angle(puerta.rotation, destino) > 0.1f)
        {
            puerta.rotation = Quaternion.Lerp(puerta.rotation, destino, Time.deltaTime * velocidadRotacion);
            yield return null;
        }
        puerta.rotation = destino; // Asegura que llega exactamente al destino
    }
}
