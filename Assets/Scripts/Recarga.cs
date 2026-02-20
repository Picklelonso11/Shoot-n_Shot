using UnityEngine;

public class Recarga : MonoBehaviour
{
    Disparo disparos;
    public int maxDisparos = 6;
    int disparosActuales;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        disparos = this.GetComponent<Disparo>();
        disparosActuales = maxDisparos;
    }

    // Update is called once per frame
    void Update()
    {

    } 

    int RestarBalas ()
    {
        --disparosActuales;
        return disparosActuales;
    }

    void SumarBalas()
    {

    }
}
