using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MirillaMovement : MonoBehaviour
{
    private bool imPlayer1;

    [SerializeField] RondaManager rondaManager;
    [SerializeField] ScoreManager scoreManager;

    [Header("Velocidad")]
    public float velocidadMax = 1100f;
    public float suavizado = 0.1f;

    [Header("Borrachera avanzada")]
    public float driftVelocidad = 0.8f;
    public float driftMax = 1.5f;

    private Vector2 driftActual = Vector2.zero;
    private int nivelBorrachera = 0;

    public RectTransform img;
    public RectTransform canvas;

    private Gamepad myGamepad;
    private Vector2 velocidadActual;
    private Vector2 velocidadRef;
    private Coroutine borracheraActiva;
    private float suavizadoBase = 0.1f;
    private MirillaBorracheraEffect boracheraEffect;

    void Start()
    {
        boracheraEffect = GetComponent<MirillaBorracheraEffect>();
        imPlayer1 = gameObject.CompareTag("Player1");

        if (Gamepad.all.Count >= 1 && imPlayer1)
            myGamepad = Gamepad.all[0];
        if (Gamepad.all.Count >= 2 && !imPlayer1)
            myGamepad = Gamepad.all[1];
    }

    void Update()
    {
        Vector2 input = ObtenerInput();

        if (nivelBorrachera >= 3 && nivelBorrachera < 5 && input != Vector2.zero)
        {
            Vector2 perpendicular = new Vector2(-input.y, input.x);
            driftActual += perpendicular * driftVelocidad * Time.unscaledDeltaTime;
            driftActual = Vector2.ClampMagnitude(driftActual, driftMax);
        }
        else
        {
            driftActual = Vector2.zero;
        }

        Vector2 direccionFinal = (input + driftActual).normalized;
        Vector2 velocidadObjetivo = direccionFinal * velocidadMax;

        velocidadActual = Vector2.SmoothDamp(
            velocidadActual, velocidadObjetivo, ref velocidadRef, suavizado,
            Mathf.Infinity, Time.unscaledDeltaTime);

        img.anchoredPosition += velocidadActual * Time.unscaledDeltaTime;
        LimitarCanvas();
    }

    private Vector2 ObtenerInput()
    {
        Vector2 stick = myGamepad != null ? myGamepad.leftStick.ReadValue() : Vector2.zero;

        if (stick == Vector2.zero)
        {
            float x = 0, y = 0;
            if (imPlayer1)
            {
                if (Keyboard.current.aKey.isPressed) x -= 1;
                if (Keyboard.current.dKey.isPressed) x += 1;
                if (Keyboard.current.wKey.isPressed) y += 1;
                if (Keyboard.current.sKey.isPressed) y -= 1;
            }
            else
            {
                if (Keyboard.current.leftArrowKey.isPressed) x -= 1;
                if (Keyboard.current.rightArrowKey.isPressed) x += 1;
                if (Keyboard.current.upArrowKey.isPressed) y += 1;
                if (Keyboard.current.downArrowKey.isPressed) y -= 1;
            }
            stick = new Vector2(x, y).normalized;
        }
        return stick;
    }

    private void LimitarCanvas()
    {
        Vector2 pos = img.anchoredPosition;
        float halfW = canvas.rect.width / 2f;
        float halfH = canvas.rect.height / 2f;
        float halfImgW = img.rect.width / 2f;
        float halfImgH = img.rect.height / 2f;
        pos.x = Mathf.Clamp(pos.x, -halfW + halfImgW, halfW - halfImgW);
        pos.y = Mathf.Clamp(pos.y, -halfH + halfImgH, halfH - halfImgH);
        img.anchoredPosition = pos;
    }

    // ── Borrachera normal (por chupitos) ──────────────────────────────────────
    public void Borrachera(int chupitosBebidos)
    {
        if (borracheraActiva != null)
            StopCoroutine(borracheraActiva);

        nivelBorrachera = chupitosBebidos;
        boracheraEffect?.SetNivelBorrachera(chupitosBebidos);

        float nuevoSuavizado = suavizadoBase;

        switch (chupitosBebidos)
        {
            case 0: nuevoSuavizado = 0.1f; borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado)); break;
            case 1: nuevoSuavizado = 0.4f; borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado)); break;
            case 2: nuevoSuavizado = 0.5f; borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado)); break;
            case 3: nuevoSuavizado = 0.7f; borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado)); break;
            case 4: nuevoSuavizado = 0.9f; borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado)); break;
            case 5:
                boracheraEffect?.SetNivelBorrachera(0);
                rondaManager.FinalizarRonda(scoreManager.score1, scoreManager.score2);
                break;
        }
    }

    // ── Borrachera máxima temporal (efecto botella) ───────────────────────────
    public void BorrachoMaximo()
    {
        if (borracheraActiva != null)
            StopCoroutine(borracheraActiva);

        nivelBorrachera = 4;
        suavizado = 0.9f;
        boracheraEffect?.SetNivelBorrachera(4);
        borracheraActiva = StartCoroutine(BorracheraTemporal(0.9f));
    }

    private IEnumerator BorracheraTemporal(float valor)
    {
        suavizado = valor;
        yield return new WaitForSeconds(10f);
        suavizado = suavizadoBase;
        nivelBorrachera = 0;
        borracheraActiva = null;
        boracheraEffect?.PararEfecto();
    }
}
