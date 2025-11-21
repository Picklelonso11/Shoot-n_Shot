using UnityEngine;
using UnityEngine.SceneManagement;

public class Objetivo : MonoBehaviour
{
    // Botella Rota
    public GameObject prefabFracturado;
    // Cantidad de puntos que vale esta botella (se asigna al ser destruida)
    public int puntos = 0;

    public void Disparado(Disparo quienDisparo)
    {

        if (quienDisparo != null)
        {
            // Accedemos al ScoreManager y sumamos los puntos al jugador correspondiente
            // "quienDisparo.imPlayer1" = Jugador 1 (true) o Jugador 2 (false)
            ScoreManager.Instance.AddScore(quienDisparo.imPlayer1, puntos);
        }
        // Instanciar la botella fracturada
        GameObject fracturado = Instantiate(prefabFracturado, transform.position, transform.rotation);
        // Aplicar físicas a cada trozo
        Rigidbody[] trozos = fracturado.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in trozos)
        {
            // Fuerza explosiva hacia afuera
            Vector3 fuerzaRandom = Random.insideUnitSphere * 3f;
            rb.AddForce(fuerzaRandom, ForceMode.Impulse);

            // Pequeña rotación aleatoria
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }

        // Destruir la botella original
        Destroy(gameObject);

        // Opcional: destruir los trozos después de un tiempo
        Destroy(fracturado, 5f);
    }
}
