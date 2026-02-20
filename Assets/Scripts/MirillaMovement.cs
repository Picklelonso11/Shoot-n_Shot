using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MirillaMovement : MonoBehaviour
{
    private bool imPlayer1;

    [SerializeField]
    RondaManager rondaManager;
    [SerializeField]
    ScoreManager scoreManager;

    [Header("Velocidad")]
    public float velocidadMax = 1100f;   // Velocidad máxima que puede alcanzar la mirilla
    public float suavizado = 0.1f;     // Tiempo de aceleración y frenado (más alto = más suave)

    [Header("Borrachera avanzada")]
    public float driftVelocidad = 0.8f;     // Qué tan rápido se tuerce
    public float driftMax = 1.5f;           // Máxima desviación permitida

    private Vector2 driftActual = Vector2.zero;
    private int nivelBorrachera = 0;

    public RectTransform img;
    public RectTransform canvas;

    private Gamepad myGamepad;

    // Velocidad real de la mirilla en este momento
    private Vector2 velocidadActual;

    // Variable interna necesaria para SmoothDamp
    private Vector2 velocidadRef;

    private Coroutine borracheraActiva;
    private float suavizadoBase = 0.1f;

    void Start()
    {
        // Decide si este objeto es Player1 o Player2
        imPlayer1 = gameObject.CompareTag("Player1");

        // Asigna el mando correspondiente
        if (Gamepad.all.Count >= 1)
        {
            if (imPlayer1)
            {
                myGamepad = Gamepad.all[0];   // Primer mando
            }
            else if (Gamepad.all.Count >= 2)
            {
                myGamepad = Gamepad.all[1];   // Segundo mando
            }
        }
    }

    void Update()
    {
        // Obtener dirección de entrada (mando o teclado)
        Vector2 input = ObtenerInput();

        // ===== DRIFT SOLO EN BORRACHERA ALTA =====
        if (nivelBorrachera >= 3 && nivelBorrachera < 5 && input != Vector2.zero)
        {
            // Dirección perpendicular aleatoria estable
            Vector2 perpendicular = new Vector2(-input.y, input.x);

            // Aumenta desviación progresivamente mientras mantienes
            driftActual += perpendicular * driftVelocidad * Time.deltaTime;

            // Limitar cuánto se tuerce
            driftActual = Vector2.ClampMagnitude(driftActual, driftMax);
        }
        else
        {
            // Al soltar o borrachera baja reiniciar desviación
            driftActual = Vector2.zero;
        }

        // Dirección final con desviación
        Vector2 direccionFinal = (input + driftActual).normalized;

        Vector2 velocidadObjetivo = direccionFinal * velocidadMax;
        // Suavizar aceleración y desaceleración
        velocidadActual = Vector2.SmoothDamp(velocidadActual, velocidadObjetivo, ref velocidadRef, suavizado);

        // Aplicar movimiento usando la velocidad suavizada
        img.anchoredPosition += velocidadActual * Time.deltaTime;

        // Evitar que salga del canvas
        LimitarCanvas();
    }

    // Obtiene el input combinando mando y teclado
    private Vector2 ObtenerInput()
    {
        // Leer stick del mando si existe
        Vector2 stick = myGamepad != null ? myGamepad.leftStick.ReadValue() : Vector2.zero;

        // Si el stick está quieto, permitir teclado
        if (stick == Vector2.zero)
        {
            float x = 0;
            float y = 0;

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

    public void Borrachera(int chupitosBebidos)
    {
        // Si ya hay una borrachera en curso, la cancelamos
        if (borracheraActiva != null)
        {
            StopCoroutine(borracheraActiva);
        }

        nivelBorrachera = chupitosBebidos;
        float nuevoSuavizado = suavizadoBase;

        switch (chupitosBebidos)
        {
            case 0: 
                nuevoSuavizado = 0.1f;
                borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado));
                break;
            case 1: 
                nuevoSuavizado = 0.2f;
                borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado));
                break;
            case 2: 
                nuevoSuavizado = 0.3f;
                borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado));
                break;
            case 3: 
                nuevoSuavizado = 0.4f;
                borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado));
                break;
            case 4: 
                nuevoSuavizado = 0.5f;
                borracheraActiva = StartCoroutine(BorracheraTemporal(nuevoSuavizado));
                break;
            case 5:
                Debug.Log("5 chupitos bebidos");
                rondaManager.FinalizarRonda(scoreManager.score1, scoreManager.score2); 
                break;
        }
    }

    private IEnumerator BorracheraTemporal(float valor)
    {
        suavizado = valor;

        yield return new WaitForSeconds(3f);

        suavizado = suavizadoBase;
        borracheraActiva = null;
    }
}

