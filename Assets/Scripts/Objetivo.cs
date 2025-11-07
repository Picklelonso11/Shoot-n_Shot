using UnityEngine;
using UnityEngine.SceneManagement;

public class Objetivo : MonoBehaviour
{
    // Define el tipo de botella (debe configurarse en el Inspector)
    public string tipoBotella;

    // Cantidad de puntos que vale esta botella (se asigna al ser destruida)
    private int puntos = 0;

    public void Disparado(Disparo quienDisparo)
    {
        // Asignamos la cantidad de puntos según el tipo de botella
        switch (tipoBotella.ToLower())
        {
            case "comun":
                puntos = 10; 
                break;
            case "raro":
                puntos = 25; 
                break;
            case "epico":
                puntos = 50; 
                break;
            case "legendario":
                puntos = 100; 
                break;
            default:
                break;
        }

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
