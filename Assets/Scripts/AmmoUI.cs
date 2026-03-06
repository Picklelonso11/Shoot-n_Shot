using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Ejemplo de UI para el sistema de munición.
/// Adjunta este script a un GameObject de UI vacío que gestione ambos jugadores.
/// </summary>
public class AmmoUI : MonoBehaviour
{
    [System.Serializable]
    public class ButtonSpriteSet
    {
        [Tooltip("Sprites en orden: botón 0, 1, 2, 3")]
        public List<Sprite> keyboard;   // Teclas 1, 2, 3, 4
        public List<Sprite> gamepad;    // Botones X, ○, △, □
    }

    [System.Serializable]
    public class PlayerUI
    {
        public TextMeshProUGUI ammoText;
        public GameObject reloadPanel;
        public List<Image> sequenceIcons;  // 4 Image components para la secuencia
        public ButtonSpriteSet spriteSet;  // Sprites separados por dispositivo

        public Color normalColor = Color.white;
        public Color activeColor = Color.yellow;
        public Color completedColor = Color.green;
    }

    [Header("Players UI")]
    public PlayerUI player1UI;
    public PlayerUI player2UI;

    PlayerUI GetPlayerUI(int playerIndex) => playerIndex == 0 ? player1UI : player2UI;

    // ── Actualizar display de balas ───────────────────────────────────────────
    public void UpdateAmmoDisplay(int current, int max, int playerIndex)
    {
        var ui = GetPlayerUI(playerIndex);
        if (ui.ammoText != null)
            ui.ammoText.text = $"{current}/{max}";
    }

    // ── Mostrar secuencia de recarga ──────────────────────────────────────────
    public void ShowReloadSequence(List<int> sequence, string[] labels, int playerIndex, InputDeviceType deviceType)
    {
        var ui = GetPlayerUI(playerIndex);
        if (ui.reloadPanel != null)
            ui.reloadPanel.SetActive(true);

        List<Sprite> sprites = GetSpritesForDevice(ui, deviceType);

        for (int i = 0; i < ui.sequenceIcons.Count && i < sequence.Count; i++)
        {
            if (ui.sequenceIcons[i] == null) continue;

            // El primero arranca en activeColor, el resto en normalColor
            ui.sequenceIcons[i].color = (i == 0) ? ui.activeColor : ui.normalColor;

            if (sprites != null && sequence[i] < sprites.Count && sprites[sequence[i]] != null)
                ui.sequenceIcons[i].sprite = sprites[sequence[i]];

            var label = ui.sequenceIcons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = labels[sequence[i]];
        }
    }

    List<Sprite> GetSpritesForDevice(PlayerUI ui, InputDeviceType deviceType)
    {
        return deviceType switch
        {
            InputDeviceType.Keyboard => ui.spriteSet.keyboard,
            InputDeviceType.NumpadKeyboard => ui.spriteSet.keyboard,
            InputDeviceType.Gamepad => ui.spriteSet.gamepad,
            _ => ui.spriteSet.keyboard
        };
    }

    // ── Resaltar paso actual ──────────────────────────────────────────────────
    public void HighlightStep(int stepCompleted, int playerIndex)
    {
        var ui = GetPlayerUI(playerIndex);
        if (stepCompleted - 1 >= 0 && stepCompleted - 1 < ui.sequenceIcons.Count)
            ui.sequenceIcons[stepCompleted - 1].color = ui.completedColor;

        if (stepCompleted < ui.sequenceIcons.Count)
            ui.sequenceIcons[stepCompleted].color = ui.activeColor;
    }

    // ── Resetear resaltado (error) ────────────────────────────────────────────
    public void ResetHighlight(int playerIndex)
    {
        var ui = GetPlayerUI(playerIndex);
        for (int i = 0; i < ui.sequenceIcons.Count; i++)
        {
            if (ui.sequenceIcons[i] != null)
                ui.sequenceIcons[i].color = (i == 0) ? ui.activeColor : ui.normalColor;
        }
    }

    // ── Ocultar panel de recarga ──────────────────────────────────────────────
    public void HideReloadSequence(int playerIndex)
    {
        var ui = GetPlayerUI(playerIndex);
        if (ui.reloadPanel != null)
            ui.reloadPanel.SetActive(false);
    }
}

