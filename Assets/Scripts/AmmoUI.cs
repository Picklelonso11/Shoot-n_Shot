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
    public class PlayerUI
    {
        public TextMeshProUGUI ammoText;
        public GameObject reloadPanel;         // Panel que aparece al quedarse sin balas
        public List<Image> sequenceIcons;      // 4 imágenes para los botones de la secuencia
        public List<Sprite> buttonSpritesP1;   // Sprites: 1,2,3,4 / Num1-4 / X,○,△,□
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
    public void ShowReloadSequence(List<int> sequence, string[] labels, int playerIndex)
    {
        var ui = GetPlayerUI(playerIndex);
        if (ui.reloadPanel != null)
            ui.reloadPanel.SetActive(true);

        // Asignar iconos/textos según la secuencia aleatoria
        for (int i = 0; i < ui.sequenceIcons.Count && i < sequence.Count; i++)
        {
            if (ui.sequenceIcons[i] != null)
            {
                ui.sequenceIcons[i].color = ui.normalColor;

                // Si usas sprites, asigna: ui.sequenceIcons[i].sprite = ui.buttonSpritesP1[sequence[i]];
                // Si usas TextMeshPro en los hijos, puedes poner el label de texto:
                var label = ui.sequenceIcons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (label != null) label.text = labels[sequence[i]];
            }
        }
    }

    // ── Resaltar paso actual ──────────────────────────────────────────────────
    public void HighlightStep(int stepCompleted, int playerIndex)
    {
        var ui = GetPlayerUI(playerIndex);
        // Marcar el paso anterior como completado
        if (stepCompleted - 1 >= 0 && stepCompleted - 1 < ui.sequenceIcons.Count)
            ui.sequenceIcons[stepCompleted - 1].color = ui.completedColor;

        // Resaltar el siguiente
        if (stepCompleted < ui.sequenceIcons.Count)
            ui.sequenceIcons[stepCompleted].color = ui.activeColor;
    }

    // ── Resetear resaltado (error) ────────────────────────────────────────────
    public void ResetHighlight(int playerIndex)
    {
        var ui = GetPlayerUI(playerIndex);
        foreach (var icon in ui.sequenceIcons)
            if (icon != null) icon.color = ui.normalColor;
    }

    // ── Ocultar panel de recarga ──────────────────────────────────────────────
    public void HideReloadSequence(int playerIndex)
    {
        var ui = GetPlayerUI(playerIndex);
        if (ui.reloadPanel != null)
            ui.reloadPanel.SetActive(false);
    }
}

