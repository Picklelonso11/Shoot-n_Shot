using UnityEngine;
using TMPro;

public class Contador : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    [SerializeField]
    RondaManager rondaManager;
    [SerializeField]
    ScoreManager scoreManager;

    public float currentTime = 20f;
    float tiempo;
    private void Awake()
    {
        tiempo = currentTime;
    }
    void Update()
    {
        if (rondaManager.RondaEnCurso())
        {
            // Restar tiempo cada frame
            tiempo -= Time.deltaTime;

            // Mostrar el número redondeado
            countdownText.text = Mathf.CeilToInt(tiempo).ToString();
            if (tiempo < 0f)
            {
                // Evitar que baje de 0
                tiempo = 0f;
                rondaManager.FinalizarRonda(scoreManager.score1, scoreManager.score2);
            }
        }
        else
        {
            tiempo = currentTime;
        }
    }
}
