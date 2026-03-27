using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona el menú principal.
/// Adjunta este script a un GameObject en la escena del menú.
/// Asigna panelInstrucciones en el Inspector.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Panel con las instrucciones del juego")]
    public GameObject panelInstrucciones;

    [Header("Escenas")]
    [Tooltip("Nombre exacto de la escena de juego")]
    public string escenaJuego = "Partida";

    void Start()
    {
        // Asegurarse de que el timeScale está a 1 por si venimos de una pausa
        Time.timeScale = 1f;
        panelInstrucciones?.SetActive(false);
    }

    // ── Botones principales ───────────────────────────────────────────────────

    public void IniciarPartida()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void VerInstrucciones()
    {
        panelInstrucciones?.SetActive(true);
    }

    public void CerrarInstrucciones()
    {
        panelInstrucciones?.SetActive(false);
    }
}
