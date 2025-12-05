using UnityEngine;
using TMPro;

public class Contador : MonoBehaviour
{
    public TextMeshProUGUI countdownText;

    // Tiempo total
    private float currentTime = 22f;

    void Update()
    {
        // Restar tiempo cada frame
        currentTime -= Time.deltaTime;

        // Evitar que baje de 0
        if (currentTime < 0f)
            currentTime = 0f;

        // Mostrar el valor redondeado en el texto
        countdownText.text = Mathf.CeilToInt(currentTime).ToString();
    }
}
