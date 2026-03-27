using UnityEngine;
using DG.Tweening;

/// <summary>
/// Adjunta este script al prefab 3D de la pistola.
/// La pistola rota siguiendo el movimiento de la mirilla
/// y hace retroceso al disparar.
/// </summary>
public class GunFollow : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform mirilla;

    [Header("Seguimiento de mirilla")]
    [Tooltip("Qué tanto rota la pistola siguiendo la mirilla (grados por unidad de canvas)")]
    public float influencia = 0.01f;
    [Tooltip("Suavizado del seguimiento (más alto = más pegado)")]
    public float suavizado = 8f;
    [Tooltip("Rotación máxima permitida en cada eje (grados)")]
    public float maxRotacion = 15f;

    [Header("Retroceso al disparar")]
    [Tooltip("Grados que sube la pistola al disparar (eje X)")]
    public float retrocesoRotX = 8f;
    [Tooltip("Duración del golpe de retroceso")]
    public float duracionRetroceso = 0.07f;
    [Tooltip("Duración del retorno a la posición original")]
    public float duracionRetorno = 0.12f;

    // Rotación base en local space
    private Quaternion rotacionBase;

    // Para evitar conflictos con DOTween durante el retroceso
    private bool enRetroceso = false;

    void Start()
    {
        rotacionBase = transform.localRotation;
    }

    void Update()
    {
        if (enRetroceso) return;

        Vector2 miriPos = mirilla.anchoredPosition;

        // Convertir posición de la mirilla a rotación
        // X de la mirilla → rotación en Y (girar izquierda/derecha)
        // Y de la mirilla → rotación en X (inclinar arriba/abajo)
        float rotY = Mathf.Clamp(miriPos.x * influencia, -maxRotacion, maxRotacion);
        float rotX = Mathf.Clamp(-miriPos.y * influencia, -maxRotacion, maxRotacion);

        Quaternion rotObjetivo = rotacionBase * Quaternion.Euler(rotX, rotY, 0f);

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            rotObjetivo,
            Time.deltaTime * suavizado
        );
    }

    /// <summary>
    /// Llama a este método desde Disparo.cs cuando se confirma un disparo válido.
    /// </summary>
    public void Retroceso()
    {
        if (enRetroceso) return;

        // Bloquear Update antes de cualquier otra cosa
        enRetroceso = true;
        transform.DOKill();

        // Calcular la rotación objetivo de seguimiento en este momento
        Vector2 miriPos = mirilla.anchoredPosition;
        float rotY = Mathf.Clamp(miriPos.x * influencia, -maxRotacion, maxRotacion);
        float rotX = Mathf.Clamp(-miriPos.y * influencia, -maxRotacion, maxRotacion);
        Quaternion rotObjetivo = rotacionBase * Quaternion.Euler(rotX, rotY, 0f);

        // Retroceso desde la rotación objetivo, no desde la interpolada
        Quaternion rotRetroceso = rotObjetivo * Quaternion.Euler(-retrocesoRotX, 0f, 0f);

        DOTween.Sequence()
            .Append(transform.DOLocalRotateQuaternion(rotRetroceso, duracionRetroceso).SetEase(Ease.OutQuad))
            .Append(transform.DOLocalRotateQuaternion(rotObjetivo, duracionRetorno).SetEase(Ease.InOutQuad))
            .OnComplete(() => enRetroceso = false);
    }
}
