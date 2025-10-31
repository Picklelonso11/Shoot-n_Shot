using UnityEngine;

public class Objetivo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Disparado(Disparo shooter)
    {
        Debug.Log("Me disparó: " + shooter.name);
        Destroy(gameObject);
    }
}
