using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.ComponentModel;
using System;
using UnityEngine.SceneManagement;

public class RondaManager : MonoBehaviour
{
    public static RondaManager Instance;
    public ScoreManager scoreManager;
    public SpawnManager spawnManager;
    public MoverChupito chupitos;

    [Header("UI")]
    public TextMeshProUGUI countdownText;      // Texto 3-2-1-GO
    public GameObject endRoundPanel;           // Panel de fin de ronda
    public TextMeshProUGUI winnerText;         // Texto ganador de ronda
    public TextMeshProUGUI j1Text;         // Texto puntos J1
    public TextMeshProUGUI j2Text;         // Texto puntos J2
    public Button nextRoundButton;             // Botón siguiente ronda
    public AudioSource horn;
    public AudioSource hornLarge;
    public AudioSource go;
    public AudioSource music;

    [Header("Pantalla final")]
    public GameObject finalGamePanel;          // Panel final
    public TextMeshProUGUI finalResultText;    // Texto final del ganador absoluto

    // Marcadores de rondas ganadas
    private int rondasGanadasJ1 = 0;
    private int rondasGanadasJ2 = 0;

    //Marcadores de ronda activa y número de ronda
    private bool rondaActiva = false;
    private int rondaActual = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Ocultar los paneles
        endRoundPanel.SetActive(false);
        finalGamePanel.SetActive(false);

        // Comienza la primera ronda
        StartCoroutine(InicioRonda());
    }

    // INICIO DE RONDA
    IEnumerator InicioRonda()
    {
        chupitos.ResetearChupitos();
        music.Stop();
        rondaActiva = false;

        Time.timeScale = 0f;

        countdownText.gameObject.SetActive(true);

        // Secuencia visual 3 - 2 - 1 - GO
        countdownText.text = "3";
        horn.Play();
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "2";
        horn.Play();
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "1";
        horn.Play();
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "GO!";
        go.Play();
        hornLarge.Play();
        yield return new WaitForSecondsRealtime(0.8f);

        countdownText.gameObject.SetActive(false);

        // Activar el juego
        Time.timeScale = 1f;

        // Añadir la música
        music.Play();

        // Se activa la ronda
        rondaActiva = true;
    }

    // FIN DE RONDA 
    public void FinalizarRonda(int puntosJ1, int puntosJ2)
    {
        if (!rondaActiva) return;
        rondaActiva = false;
        spawnManager.DestruirBotellaActual();
        music.Stop();

        // Determinar ganador de la ronda
        if (puntosJ1 > puntosJ2)
        {
            rondasGanadasJ1++;
            winnerText.color = Color.red;
            winnerText.text = "Ronda de J1";
        }
        else if (puntosJ2 > puntosJ1)
        {
            rondasGanadasJ2++;
            winnerText.color = new Color(0.2f, 0.8f, 1f); 
            winnerText.text = "Ronda de J2";
        }
        else
        {
            winnerText.color = Color.grey;
            winnerText.text = "Empate";
        }
        j1Text.text = Convert.ToString(puntosJ1);
        j2Text.text = Convert.ToString(puntosJ2);

        // Mostrar panel de fin de ronda
        endRoundPanel.SetActive(true);

        // Configurar botón
        nextRoundButton.onClick.RemoveAllListeners();
        nextRoundButton.onClick.AddListener(SiguienteRonda);

        // Marcamos la siguiente ronda
        rondaActual++;
    }

    // SIGUIENTE RONDA / O FIN DEL JUEGO
    void SiguienteRonda()
    {
        endRoundPanel.SetActive(false);

        // ¿Alguien llegó a 3 rondas ganadas?
        if (rondasGanadasJ1 >= 3 || rondasGanadasJ2 >= 3)
        {
            music.Stop();
            MostrarGanadorFinal();
            return;
        }

        // Si no, inicia otra ronda
        StartCoroutine(InicioRonda());
    }

    // PANTALLA FINAL
    void MostrarGanadorFinal()
    {
        finalGamePanel.SetActive(true);

        if (rondasGanadasJ1 > rondasGanadasJ2)
        {
            finalResultText.text = "Victoria de J1\n" + "Rondas J1: " + rondasGanadasJ1 + "\n" + "Rondas J2: " + rondasGanadasJ2;
        }
        else
        {
            finalResultText.text = "Victoria de J2\n" + "Rondas J1: " + rondasGanadasJ1 + "\n" + "Rondas J2: " + rondasGanadasJ2;
        }
    }

    public bool RondaEnCurso()
    {
        return rondaActiva;
    }

    // Método para reiniciar la escena actual
    public void ReiniciarEscena()
    {
        // Obtiene el nombre de la escena activa y la recarga
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Método para salir de la escena actual
    public void SalirJuego()
    {
        Application.Quit(); // Cierra la aplicación en builds
    }
    public int RondaActual()
    {
        return rondaActual; // 1, 2 o 3
    }
}
