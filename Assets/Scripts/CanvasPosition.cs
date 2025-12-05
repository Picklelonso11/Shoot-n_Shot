using TMPro;
using UnityEngine;
using System.Collections;

public class CanvasPosition : MonoBehaviour
{
    public GameObject textoPuntuacionBotella;

    public void MostrarTextoPuntuacion(Vector3 position, int puntuacion)
    {
        // Convertir posición 3D a pantalla
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Instanciar texto en el Canvas
        GameObject textoObj = Instantiate(textoPuntuacionBotella, Camera.main.WorldToScreenPoint(position), Quaternion.identity, transform);

        // Posicionar sobre la botella
        RectTransform rt = textoObj.GetComponent<RectTransform>();
        //rt.position = screenPos;

        // Escala inicial para animación
        rt.localScale = Vector3.one * 0.1f;

        // Obtener TMP
        TextMeshProUGUI tmp = textoObj.GetComponent<TextMeshProUGUI>();
        tmp.text = "+" + puntuacion;

        // COLOR / TAMAÑO / GRADIENTE
        switch (puntuacion)
        {
            case 10:
                tmp.color = Color.green;
                tmp.fontSize = 65;
                break;

            case 25:
                tmp.color = new Color(0.2f, 0.8f, 1f);
                tmp.fontSize = 75;
                break;

            case 50:
                tmp.color = new Color(1f, 0.6f, 0.2f);
                tmp.fontSize = 85;
                break;

            case 75:
                tmp.fontSize = 115;

                tmp.enableVertexGradient = true;

                tmp.colorGradient = new VertexGradient
                (
                    new Color(1f, 0.9f, 0.3f), // top left (oro claro)
                    new Color(1f, 0.8f, 0.2f), // top right (oro medio)
                    new Color(0.9f, 0.7f, 0.1f), // bottom left (oro profundo)
                    new Color(1f, 1f, 0.5f)  // bottom right (reflejo luminoso)
                );
                break;
        }

        // Iniciar animación de escala 0.1 a 1 en 0.5s
        StartCoroutine(AnimarEscala(rt));

        // Destruir después
        Destroy(textoObj, 1f);
    }
    private IEnumerator AnimarEscala(RectTransform rt)
    {
        Vector3 inicio = Vector3.one * 0.1f;
        Vector3 fin = Vector3.one;
        float duracion = 0.5f;
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
