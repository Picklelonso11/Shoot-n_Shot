using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreTextJ1;
    public TextMeshProUGUI scoreTextJ2;

    private int scoreJ1 = 0;
    private int scoreJ2 = 0;

    void Awake()
    {
        // si ya existe un ScoreManager destruye el nuevo
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para sumar puntos a un jugador
    public void AddScore(bool player1, int puntos)
    {
        // Si el disparo fue del jugador 1
        if (player1)
        {
            scoreJ1 += puntos;
            scoreTextJ1.text = "J1: " + scoreJ1;
        }
        // Si el disparo fue del jugador 2
        else
        {
            scoreJ2 += puntos;
            scoreTextJ2.text = "J2: " + scoreJ2;
        }
    }
}
