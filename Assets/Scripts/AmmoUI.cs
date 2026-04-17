using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AmmoUI : MonoBehaviour
{
    [System.Serializable]
    public class ButtonSpriteSet
    {
        [Tooltip("Sprites en orden: botón 0, 1, 2, 3")]
        public List<Sprite> keyboard;   // Teclas W, D, S, A
        public List<Sprite> arrows;     // Flechas ↑, →, ↓, ←
        public List<Sprite> gamepad;    // Botones X, ○, △, □
    }

    [System.Serializable]
    public class AmmoSpriteSet
    {
        [Tooltip("Sprites en orden: 0/6, 1/6, 2/6, 3/6, 4/6, 5/6, 6/6")]
        public List<Sprite> J1;   // Red
        public List<Sprite> J2;   // Blue
    }

    [System.Serializable]
    public class PlayerUI
    {
        public GameObject reloadPanel;
        public List<Image> sequenceIcons;  // 4 Image components para la secuencia
        public ButtonSpriteSet spriteSet;  // Sprites separados por dispositivo
        public Image reloadImage;          // Imagen del tambor/cargador
        public AmmoSpriteSet reloadSprite; // Sprites del cargador por nº de balas

        public Color normalColor = Color.white;
        public Color activeColor = Color.yellow;
        public Color completedColor = Color.green;
    }

    [Header("Players UI")]
    public PlayerUI player1UI;
    public PlayerUI player2UI;

    [Header("Animación tambor")]
    [Tooltip("Grados que rota el tambor al disparar o recargar")]
    public float drumRotationDegrees = 60f;
    [Tooltip("Duración total del giro (segundos)")]
    public float drumRotationDuration = 0.15f;


    PlayerUI GetPlayerUI(int playerIndex) => playerIndex == 0 ? player1UI : player2UI;

    // ── Actualizar display de balas ───────────────────────────────────────────
    public void UpdateAmmoDisplay(int current, int max, int playerIndex, bool isReload = false)
    {
        var ui = GetPlayerUI(playerIndex);

        if (ui.reloadImage != null && ui.reloadSprite != null)
        {
            List<Sprite> sprites = playerIndex == 0 ? ui.reloadSprite.J1 : ui.reloadSprite.J2;

            if (sprites != null && sprites.Count > 0)
            {
                int index = Mathf.Clamp(current, 0, sprites.Count - 1);
                Sprite newSprite = sprites[index];

                if (newSprite != null)
                {
                    if (isReload)
                        AnimateReload(ui.reloadImage, newSprite);
                    else
                        AnimateShoot(ui.reloadImage, newSprite);
                }
            }
        }
    }

    // ── Animación disparo: sprite cambia instantáneamente a 60° → vuelve a 0° ──
    void AnimateShoot(Image img, Sprite newSprite)
    {
        img.rectTransform.DOKill();

        // Cambiar sprite y rotación instantáneamente
        img.sprite = newSprite;
        img.rectTransform.localEulerAngles = new Vector3(0, 0, drumRotationDegrees);

        // Animar volviendo a 0°
        img.rectTransform.DORotate(
            Vector3.zero,
            drumRotationDuration, RotateMode.Fast).SetEase(Ease.OutQuad);
    }

    // ── Animación recarga: rota de 0° a 60° → cambia sprite → vuelve a 0° instantáneo ──
    void AnimateReload(Image img, Sprite newSprite)
    {
        img.rectTransform.DOKill();

        // Asegurarse de que parte desde 0°
        img.rectTransform.localEulerAngles = Vector3.zero;

        DOTween.Sequence()
            .Append(img.rectTransform.DORotate(
                new Vector3(0, 0, drumRotationDegrees),
                drumRotationDuration, RotateMode.Fast).SetEase(Ease.InQuad))
            .AppendCallback(() =>
            {
                img.sprite = newSprite;
                img.rectTransform.localEulerAngles = Vector3.zero;
            });
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
            InputDeviceType.Keyboard => ui.spriteSet.arrows,
            InputDeviceType.NumpadKeyboard => ui.spriteSet.arrows,
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
