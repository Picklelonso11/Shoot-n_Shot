using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Detecta cuántos mandos hay conectados y asigna automáticamente
/// el tipo de input y el gamepad correspondiente a cada AmmoSystem.
/// Coloca este script en un GameObject persistente (ej. GameManager).
/// </summary>
public class InputDeviceManager : MonoBehaviour
{
    [Header("Referencias a los jugadores")]
    public Ammunition player1;
    public Ammunition player2;

    void OnEnable()
    {
        // Suscribirse a eventos de conexión/desconexión de mandos
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void Start()
    {
        AssignInputDevices();
    }

    // Se llama automáticamente cuando se conecta o desconecta un mando
    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added ||
                change == InputDeviceChange.Removed ||
                change == InputDeviceChange.Reconnected ||
                change == InputDeviceChange.Disconnected)
            {
                AssignInputDevices();
            }
        }
    }

    public void AssignInputDevices()
    {
        List<Gamepad> gamepads = new List<Gamepad>(Gamepad.all);
        int gamepadCount = gamepads.Count;

        Debug.Log($"[InputDeviceManager] Mandos detectados: {gamepadCount}");

        if (gamepadCount >= 2)
        {
            // ── 2 o más mandos: ambos jugadores usan gamepad ──────────────────
            SetupPlayer(player1, InputDeviceType.Gamepad, gamepads[0]);
            SetupPlayer(player2, InputDeviceType.Gamepad, gamepads[1]);

            Debug.Log("Modo: Gamepad P1 | Gamepad P2");
        }
        else if (gamepadCount == 1)
        {
            // ── 1 mando: P1 = gamepad, P2 = teclado numérico ─────────────────
            SetupPlayer(player1, InputDeviceType.Gamepad, gamepads[0]);
            SetupPlayer(player2, InputDeviceType.NumpadKeyboard, null);

            Debug.Log("Modo: Gamepad P1 | NumpadKeyboard P2");
        }
        else
        {
            // ── 0 mandos: P1 = teclado normal, P2 = teclado numérico ─────────
            SetupPlayer(player1, InputDeviceType.Keyboard, null);
            SetupPlayer(player2, InputDeviceType.NumpadKeyboard, null);

            Debug.Log("Modo: Keyboard P1 | NumpadKeyboard P2");
        }
    }

    void SetupPlayer(Ammunition player, InputDeviceType deviceType, Gamepad gamepad)
    {
        if (player == null) return;

        player.inputDevice = deviceType;
        player.assignedGamepad = gamepad;
    }

    // ── Utilidad: número de mandos conectados (acceso externo) ────────────────
    public static int GetConnectedGamepadCount() => Gamepad.all.Count;
}
