using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[HideInInspector]
public enum InputDeviceType { Keyboard, NumpadKeyboard, Gamepad }

public class Ammunition : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 6;
    public float reloadInterval = 1f; // Una bala cada X segundos

    [Header("Player Settings")]
    public int playerIndex = 0; // 0 = Player1, 1 = Player2
    public InputDeviceType inputDevice = InputDeviceType.Keyboard;

    // Estado de balas
    private int currentAmmo;
    private float reloadTimer;

    // Estado de recarga manual
    private bool isManualReloading = false;
    private List<int> reloadSequence = new List<int>(); // índices de botones (0-3)
    private int currentReloadStep = 0;

    // UI Reference
    [Header("UI")]
    public AmmoUI ammoUI; // Asigna tu script de UI aquí

    // Botones según dispositivo
    private string[] keyboardLabels = { "1", "2", "3", "4" };
    private string[] numpadLabels = { "Num1", "Num2", "Num3", "Num4" };
    private string[] gamepadLabels = { "X", "○", "△", "□" };

    // ─── Teclas reales ───────────────────────────────────────────────────────
    private Key[] keyboardKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4 };
    private Key[] numpadKeys = { Key.Numpad1, Key.Numpad2, Key.Numpad3, Key.Numpad4 };

    // Botones de gamepad (requiere que el Gamepad esté asignado al player)
    private GamepadButton[] gamepadButtons =
    {
        GamepadButton.DpadDown,  // X (PS) / A (Xbox)
        GamepadButton.DpadRight,   // ○ (PS) / B (Xbox)
        GamepadButton.DpadUp,  // △ (PS) / Y (Xbox)
        GamepadButton.DpadLeft    // □ (PS) / X (Xbox)
    };

    // Gamepad asignado a este jugador (configura desde PlayerInputManager o manualmente)
    [HideInInspector] public Gamepad assignedGamepad;

    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        currentAmmo = maxAmmo;
        reloadTimer = reloadInterval;
        isManualReloading = false;
        ammoUI?.HideReloadSequence(playerIndex);
        UpdateUI();
    }

    void Update()
    {
        HandleAutoReload();

        if (isManualReloading)
            HandleManualReloadInput();
    }

    // ── AUTO-RECARGA ──────────────────────────────────────────────────────────
    void HandleAutoReload()
    {
        if (currentAmmo >= maxAmmo) return;
        if (isManualReloading) return; // Si está en recarga manual, pausar auto-recarga

        reloadTimer -= Time.deltaTime;
        if (reloadTimer <= 0f)
        {
            currentAmmo++;
            currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
            reloadTimer = reloadInterval;
            UpdateUI();
        }
    }

    // ── DISPARAR ──────────────────────────────────────────────────────────────
    public bool TryShoot()
    {
        if (isManualReloading) return false;
        if (currentAmmo <= 0)
        {
            // No hay balas - esto no debería pasar porque al llegar a 0 se activa la recarga
            return false;
        }

        currentAmmo--;
        reloadTimer = reloadInterval; // Resetear timer al disparar
        UpdateUI();

        if (currentAmmo <= 0)
            StartManualReload();

        return true;
    }

    // ── RECARGA MANUAL ────────────────────────────────────────────────────────
    void StartManualReload()
    {
        isManualReloading = true;
        currentReloadStep = 0;
        reloadSequence = GenerateRandomSequence(4); // Secuencia de 4 botones (índices 0-3)

        ammoUI?.ShowReloadSequence(reloadSequence, GetButtonLabels(), playerIndex, inputDevice);
        Debug.Log($"[Player {playerIndex + 1}] ¡Recarga manual! Secuencia: {string.Join(", ", GetSequenceLabels())}");
    }

    List<int> GenerateRandomSequence(int length)
    {
        List<int> available = new List<int> { 0, 1, 2, 3 };
        List<int> seq = new List<int>();
        for (int i = 0; i < length; i++)
        {
            int r = Random.Range(0, available.Count);
            seq.Add(available[r]);
            available.RemoveAt(r);
        }
        return seq;
    }

    void HandleManualReloadInput()
    {
        for (int i = 0; i < 4; i++)
        {
            if (WasPressedThisFrame(i))
            {
                if (i == reloadSequence[currentReloadStep])
                {
                    currentReloadStep++;
                    ammoUI?.HighlightStep(currentReloadStep, playerIndex);

                    if (currentReloadStep >= reloadSequence.Count)
                        FinishManualReload();
                }
                else
                {
                    // Botón equivocado - reiniciar secuencia
                    currentReloadStep = 0;
                    ammoUI?.ResetHighlight(playerIndex);
                    Debug.Log($"[Player {playerIndex + 1}] ¡Botón incorrecto! Reiniciando secuencia.");
                }
                break;
            }
        }
    }

    bool WasPressedThisFrame(int buttonIndex)
    {
        switch (inputDevice)
        {
            case InputDeviceType.Keyboard:
                return Keyboard.current != null && Keyboard.current[keyboardKeys[buttonIndex]].wasPressedThisFrame;

            case InputDeviceType.NumpadKeyboard:
                return Keyboard.current != null && Keyboard.current[numpadKeys[buttonIndex]].wasPressedThisFrame;

            case InputDeviceType.Gamepad:
                if (assignedGamepad == null) assignedGamepad = Gamepad.current;
                return assignedGamepad != null && assignedGamepad[gamepadButtons[buttonIndex]].wasPressedThisFrame;
        }
        return false;
    }

    void FinishManualReload()
    {
        isManualReloading = false;
        currentAmmo = maxAmmo;
        reloadTimer = reloadInterval;
        ammoUI?.HideReloadSequence(playerIndex);
        UpdateUI();
        Debug.Log($"[Player {playerIndex + 1}] ¡Recarga completada! Balas: {currentAmmo}/{maxAmmo}");
    }

    // ── HELPERS ───────────────────────────────────────────────────────────────
    public string[] GetButtonLabels()
    {
        return inputDevice switch
        {
            InputDeviceType.Keyboard => keyboardLabels,
            InputDeviceType.NumpadKeyboard => numpadLabels,
            InputDeviceType.Gamepad => gamepadLabels,
            _ => keyboardLabels
        };
    }

    List<string> GetSequenceLabels()
    {
        string[] labels = GetButtonLabels();
        List<string> result = new List<string>();
        foreach (int idx in reloadSequence)
            result.Add(labels[idx]);
        return result;
    }

    void UpdateUI()
    {
        ammoUI?.UpdateAmmoDisplay(currentAmmo, maxAmmo, playerIndex);
    }

    // Propiedades públicas para UI/otros scripts
    public int CurrentAmmo => currentAmmo;
    public bool IsManualReloading => isManualReloading;
    public List<int> ReloadSequence => reloadSequence;
    public int CurrentReloadStep => currentReloadStep;
}