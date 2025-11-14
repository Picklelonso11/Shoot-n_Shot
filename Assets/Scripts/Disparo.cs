using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class Disparo : MonoBehaviour
{
    [HideInInspector] public bool imPlayer1;
    public RawImage mirilla;
    public Camera camara;
    private Gamepad myGamepad;

    void Start()
    {
        // Identificar jugador
        imPlayer1 = gameObject.CompareTag("Player1");

        // Asignar mando según tag
        if (Gamepad.all.Count >= 1 && imPlayer1)
        {
            myGamepad = Gamepad.all[0]; // Mando J1
        }
        if (Gamepad.all.Count >= 2 && !imPlayer1)
        {
            myGamepad = Gamepad.all[1]; // Mando J2
        }
    }
    void Update()
    {
        if (imPlayer1)
        {
            DisparoJugador(true);
        }
        else
        {
            DisparoJugador(false);
        }
    }
    private void AspectoMirilla()
    {
        RectTransform rt = mirilla.rectTransform;   // Si mirilla es RawImage
        RawImage img = mirilla;                     // acceso al color

        Vector3 originalScale = rt.localScale;

        Sequence seq = DOTween.Sequence();

        // Primera mitad: más pequeña + más oscura
        seq.Append(rt.DOScale(1.5f, 0.05f).SetEase(Ease.OutQuad));
        seq.Join(img.DOColor(new Color(0.35f, 0.35f, 0.35f, 1f), 0.05f));

        // Segunda mitad: vuelve a la escala y al color original
        seq.Append(rt.DOScale(originalScale, 0.05f).SetEase(Ease.InQuad));
        seq.Join(img.DOColor(Color.white, 0.05f));
    }
    private void DisparoJugador(bool player1)
    {
        bool disparo = false;

        // Entrada mando
        if (myGamepad != null && myGamepad.buttonSouth.wasPressedThisFrame)
        {
            disparo = true;
            AspectoMirilla();
        }

        // Entrada teclado (backup)
        if (!disparo)
        {
            if (player1 && Keyboard.current.leftShiftKey.wasPressedThisFrame)
            {
                disparo = true;
                AspectoMirilla();
            }
            if (!player1 && Keyboard.current.rightShiftKey.wasPressedThisFrame)
            {
                disparo = true;
                AspectoMirilla();
            }
        }

        if (!disparo)
        {
            return;
        }
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, mirilla.rectTransform.position);
        Ray ray = camara.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Botella"))
            {
                Objetivo target = hit.collider.GetComponent<Objetivo>();
                target.Disparado(this);
            }
            else
            {
                Debug.Log("Disparo fallido");
            }
        }
    }
}
