using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la pausa del juego.
/// Adjunta este script a un GameObject persistente en la escena de juego.
/// Asigna pauseMenuPanel en el Inspector.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pauseMenuPanel;

    [Header("Mandos")]
    public Gamepad gamepadJ1;
    public Gamepad gamepadJ2;

    [Header("Escenas")]
    [Tooltip("Nombre exacto de la escena del menú principal")]
    public string escenaMenuPrincipal = "MenuPrincipal";
    [Tooltip("Nombre exacto de la escena de juego para reiniciar")]
    public string escenaJuego = "Game";

    private bool pausado = false;

    void Start()
    {
        // Asignar mandos si están conectados
        if (Gamepad.all.Count >= 1) gamepadJ1 = Gamepad.all[0];
        if (Gamepad.all.Count >= 2) gamepadJ2 = Gamepad.all[1];

        pauseMenuPanel?.SetActive(false);
    }

    void Update()
    {
        // Teclado: Escape
        bool teclado = Keyboard.current != null &&
                       Keyboard.current.spaceKey.wasPressedThisFrame;

        // Gamepad J1: Options / Start
        bool padJ1 = gamepadJ1 != null &&
                     gamepadJ1.startButton.wasPressedThisFrame;

        // Gamepad J2: Options / Start
        bool padJ2 = gamepadJ2 != null &&
                     gamepadJ2.startButton.wasPressedThisFrame;

        if (teclado || padJ1 || padJ2)
            TogglePausa();
    }

    // ── Toggle pausa ──────────────────────────────────────────────────────────
    public void TogglePausa()
    {
        pausado = !pausado;
        Time.timeScale = pausado ? 0f : 1f;
        pauseMenuPanel?.SetActive(pausado);
    }

    // ── Botones del menú (asigna en el Inspector a cada Button.onClick) ───────
    public void Reanudar()
    {
        pausado = false;
        Time.timeScale = 1f;
        pauseMenuPanel?.SetActive(false);
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaJuego);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenuPrincipal);
    }
}
