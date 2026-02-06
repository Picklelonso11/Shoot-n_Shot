using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    [SerializeField]
    RondaManager rondaManager;
    public MoverChupito mover;

    public TextMeshProUGUI scoreTextJ1;
    public TextMeshProUGUI scoreTextJ2;

    public int score1 = 0;
    public int score2 = 0;

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
    private void Update()
    {
        if(scoreJ1 >= 150)
        {
            scoreJ1 = scoreJ1 - 150;
            mover.MoverSiguiente(MoverChupito.TipoObjeto.TipoJ1);
        }
        if(scoreJ2 >= 150)
        {
            scoreJ2 = scoreJ2 - 150;
            mover.MoverSiguiente(MoverChupito.TipoObjeto.TipoJ2);
        }
        if (!rondaManager.RondaEnCurso())
        {
            score1 = 0;
            score2 = 0;
            scoreJ1 = 0;
            scoreJ2 = 0;
            scoreTextJ1.text = "J1:" + score1;
            scoreTextJ2.text = "J2:" + score2;
        }
    }

    // Método para sumar puntos a un jugador
    public void AddScore(bool player1, int puntos)
    {
        // Si el disparo fue del jugador 1
        if (player1)
        {
            scoreJ1 += puntos;
            score1 += puntos;
            scoreTextJ1.text = "J1:" + score1;
        }
        // Si el disparo fue del jugador 2
        else
        {
            scoreJ2 += puntos;
            score2 += puntos;
            scoreTextJ2.text = "J2:" + score2;
        }
    }
}
