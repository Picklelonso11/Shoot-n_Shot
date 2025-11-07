using UnityEngine;
using UnityEngine.SceneManagement;

public class Objetivo : MonoBehaviour
{
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

        // Destruye la botella que fue disparada
        Destroy(gameObject);
    }
}
