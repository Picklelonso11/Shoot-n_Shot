
using UnityEngine;


public class Objetivo : MonoBehaviour
{
    // Botella Rota
    public GameObject prefabFracturado;

    public GameObject textoPuntuacionBotella;
    public Canvas canvasUI;                     // Referencia al Canvas del HUD

    // Cantidad de puntos que vale esta botella (se asigna al ser destruida)
    public int puntos = 0;

    public void Disparado(Disparo quienDisparo)
    {

        if (quienDisparo != null)
        {
            // Accedemos al ScoreManager y sumamos los puntos al jugador correspondiente
            // "quienDisparo.imPlayer1" = Jugador 1 (true) o Jugador 2 (false)
            ScoreManager.Instance.AddScore(quienDisparo.imPlayer1, puntos);
        }
        // Instanciar la botella fracturada
        GameObject fracturado = Instantiate(prefabFracturado, transform.position, transform.rotation);
        // Aplicar físicas a cada trozo
        Rigidbody[] trozos = fracturado.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in trozos)
        {
            // Fuerza explosiva hacia afuera
            Vector3 fuerzaRandom = Random.insideUnitSphere * 3f;
            rb.AddForce(fuerzaRandom, ForceMode.Impulse);

            // Pequeña rotación aleatoria
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }

        // Destruir la botella original
        Destroy(gameObject);

        // Destruir los trozos después de un tiempo
        Destroy(fracturado, 5f);
        // Texto de puntuación en la pantalla
       // MostrarTextoPuntuacion();
    }
    /*
    private void MostrarTextoPuntuacion()
    {
        // Convertir posición 3D a pantalla
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Instanciar texto en el Canvas
        GameObject textoObj = Instantiate(textoPuntuacionBotella, canvasUI.transform);

        // Posicionar sobre la botella
        RectTransform rt = textoObj.GetComponent<RectTransform>();
        rt.position = screenPos;

        // Escala inicial para animación
        rt.localScale = Vector3.one * 0.1f;

        // Obtener TMP
        TextMeshProUGUI tmp = textoObj.GetComponent<TextMeshProUGUI>();
        tmp.text = "+" + puntos;

        // COLOR / TAMAÑO / GRADIENTE
        switch (puntos)
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
    */
}
