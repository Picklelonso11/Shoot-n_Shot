using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Adjunta este script al mismo GameObject que MirillaMovement.
/// Llama a SetNivelBorrachera(nivel) para activar el efecto
/// y a PararEfecto() cuando la borrachera termina.
/// </summary>
public class MirillaBorracheraEffect : MonoBehaviour
{
    [Header("Referencias")]
    public RawImage mirilla;

    [Header("Efecto de oscilación")]
    public float escalaBase = 1f;
    public float duracionCiclo = 0.4f;  // Duración de cada medio ciclo (ida)
    public float duracionRetorno = 0.3f; // Duración del retorno al estado normal

    // Configuración por nivel (0-4)
    // { estiramiento X, estiramiento Z, rotacion }
    private static readonly float[,] nivelConfig = new float[,]
    {
        // estiX,  estiZ,  rot
        { 0f,     0f,     0f   }, // nivel 0 - sin efecto
        { 0.06f,  0.04f,  1.5f }, // nivel 1 - leve
        { 0.10f,  0.07f,  3f   }, // nivel 2 - moderado
        { 0.15f,  0.10f,  5f   }, // nivel 3 - fuerte
        { 0.22f,  0.14f,  8f   }, // nivel 4 - muy fuerte
    };

    private int nivelActual = 0;
    private Sequence secuenciaActiva;

    void Start() { }

    public void SetNivelBorrachera(int nivel)
    {
        nivelActual = Mathf.Clamp(nivel, 0, 4);
        AplicarEfecto();
    }

    /// <summary>
    /// Llamar desde MirillaMovement al terminar BorracheraTemporal.
    /// </summary>
    public void PararEfecto()
    {
        secuenciaActiva?.Kill();
        mirilla.rectTransform.DOKill();

        // Volver suavemente al estado original
        mirilla.rectTransform
            .DOScale(Vector3.one * escalaBase, duracionRetorno)
            .SetEase(Ease.OutElastic);

        mirilla.rectTransform
            .DOLocalRotate(Vector3.zero, duracionRetorno)
            .SetEase(Ease.OutSine);
    }

    void AplicarEfecto()
    {
        secuenciaActiva?.Kill();
        mirilla.rectTransform.DOKill();

        if (nivelActual == 0)
        {
            PararEfecto();
            return;
        }

        float estiX = nivelConfig[nivelActual, 0];
        float estiZ = nivelConfig[nivelActual, 1];
        float ampRot = nivelConfig[nivelActual, 2];

        // Escala estirada: X crece, Z decrece y viceversa (efecto goma)
        Vector3 escalaA = new Vector3(escalaBase + estiX, escalaBase - estiZ, 1f);
        Vector3 escalaB = new Vector3(escalaBase - estiZ, escalaBase + estiX, 1f);

        secuenciaActiva = DOTween.Sequence().SetLoops(-1, LoopType.Yoyo);

        secuenciaActiva
            .Append(mirilla.rectTransform
                .DOScale(escalaA, duracionCiclo)
                .SetEase(Ease.InOutSine))
            .Join(mirilla.rectTransform
                .DOLocalRotate(new Vector3(0, 0, ampRot), duracionCiclo)
                .SetEase(Ease.InOutSine))
            .Append(mirilla.rectTransform
                .DOScale(escalaB, duracionCiclo)
                .SetEase(Ease.InOutSine))
            .Join(mirilla.rectTransform
                .DOLocalRotate(new Vector3(0, 0, -ampRot), duracionCiclo)
                .SetEase(Ease.InOutSine));
    }

    void OnDestroy()
    {
        secuenciaActiva?.Kill();
        mirilla?.rectTransform.DOKill();
    }
}
