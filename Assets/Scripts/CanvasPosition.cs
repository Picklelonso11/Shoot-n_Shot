using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasPosition : MonoBehaviour
{
    public GameObject textoPuntuacionBotella;
    public RawImage penalizacion;

    public void MostrarTextoPuntuacion(Vector3 position, int puntos, bool esJ1, EfectoBotella efecto)
    {
        // Instanciar texto
        GameObject textoObj = Instantiate(
            textoPuntuacionBotella,
            Camera.main.WorldToScreenPoint(position),
            Quaternion.identity,
            transform);

        RectTransform rt = textoObj.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.1f;

        TextMeshProUGUI tmp = textoObj.GetComponent<TextMeshProUGUI>();
        tmp.fontSize = 170;

        // ── Texto ────────────────────────────────────────────────────────────
        switch (efecto)
        {
            case EfectoBotella.BorrachoRival:
            case EfectoBotella.BorrachoPropio:
                tmp.text = "¡BORRACHO!";
                break;
            case EfectoBotella.RestarChupito:
            case EfectoBotella.RestarChupitoProp:
                tmp.text = "-1 CHUPITO";
                break;
            case EfectoBotella.VaciarMuniRival:
            case EfectoBotella.VaciarMuniPropia:
                tmp.text = "¡SIN BALAS!";
                break;
            default:
                tmp.text = puntos >= 0 ? "+" + puntos : "" + puntos;
                break;
        }

        // ── Color: rojo si suma J1, azul si suma J2 ──────────────────────────
        // Para efectos que afectan al rival, el "beneficiado" es quien dispara
        // Para efectos que afectan al propio, el color es del rival (le perjudica)
        bool beneficiadoEsJ1 = efecto == EfectoBotella.BorrachoPropio ||
                               efecto == EfectoBotella.RestarChupitoProp ||
                               efecto == EfectoBotella.VaciarMuniPropia
                               ? !esJ1   // le perjudica al que dispara → color del rival
                               : esJ1;   // suma/perjudica al rival → color del que dispara

        tmp.color = beneficiadoEsJ1
            ? new Color(0.9f, 0.15f, 0.15f)   // Rojo J1
            : new Color(0.15f, 0.4f, 0.9f);   // Azul J2

        StartCoroutine(AnimarEscala(rt));
        Destroy(textoObj, 1f);
    }

    private IEnumerator AnimarEscala(RectTransform rt)
    {
        Vector3 inicio = Vector3.one * 0.1f;
        Vector3 fin = Vector3.one;
        float duracion = 0.2f;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            rt.localScale = Vector3.Lerp(inicio, fin, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        rt.localScale = fin;
    }
}
