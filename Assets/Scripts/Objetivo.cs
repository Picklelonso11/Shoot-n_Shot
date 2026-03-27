using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

public enum EfectoBotella
{
    Normal,          // Suma puntos al que dispara
    BorrachoRival,   // Pone borracho al máximo al rival
    RestarChupito,   // Resta un chupito al rival
    VaciarMuniRival, // Deja la munición del rival a 0
    BorrachoPropio,  // Pone borracho al máximo al que dispara
    RestarChupitoProp, // Resta un chupito al que dispara
    VaciarMuniPropia,  // Deja la munición del que dispara a 0
}

public class Objetivo : MonoBehaviour
{
    public GameObject prefabFracturado;
    public int puntos = 0;
    public EfectoBotella efecto = EfectoBotella.Normal;

    [HideInInspector] public SpawnPuerta spawnPuerta;

    // Referencias necesarias para efectos especiales
    // Se asignan desde SpawnManager al instanciar
    [HideInInspector] public MirillaMovement mirillaJ1;
    [HideInInspector] public MirillaMovement mirillaJ2;
    [HideInInspector] public Ammunition ammoJ1;
    [HideInInspector] public Ammunition ammoJ2;
    [HideInInspector] public MoverChupito moverChupito;

    private void Start()
    {
        GameObject.FindAnyObjectByType<Objetivo>();
    }

    public void Disparado(Disparo quienDisparo)
    {
        if (quienDisparo != null)
        {
            bool esJ1 = quienDisparo.imPlayer1;

            // ── Puntos ───────────────────────────────────────────────────────
            if (puntos != 0)
                ScoreManager.Instance.AddScore(esJ1, puntos);

            // ── Efectos especiales ───────────────────────────────────────────
            MirillaMovement mirillaRival = esJ1 ? mirillaJ2 : mirillaJ1;
            MirillaMovement mirillaPropia = esJ1 ? mirillaJ1 : mirillaJ2;
            Ammunition ammoRival = esJ1 ? ammoJ2 : ammoJ1;
            Ammunition ammoPropia = esJ1 ? ammoJ1 : ammoJ2;
            MoverChupito.TipoObjeto tipoRival = esJ1
                ? MoverChupito.TipoObjeto.TipoJ2
                : MoverChupito.TipoObjeto.TipoJ1;
            MoverChupito.TipoObjeto tipoPropio = esJ1
                ? MoverChupito.TipoObjeto.TipoJ1
                : MoverChupito.TipoObjeto.TipoJ2;

            switch (efecto)
            {
                case EfectoBotella.BorrachoRival:
                    mirillaRival?.BorrachoMaximo();
                    break;

                case EfectoBotella.RestarChupito:
                    moverChupito?.RestarChupito(tipoRival);
                    break;

                case EfectoBotella.VaciarMuniRival:
                    ammoRival?.VaciarMunicion();
                    break;

                case EfectoBotella.BorrachoPropio:
                    mirillaPropia?.BorrachoMaximo();
                    break;

                case EfectoBotella.RestarChupitoProp:
                    moverChupito?.RestarChupito(tipoPropio);
                    break;

                case EfectoBotella.VaciarMuniPropia:
                    ammoPropia?.VaciarMunicion();
                    break;
            }
        }

        Romper();
        FindAnyObjectByType<CanvasPosition>().MostrarTextoPuntuacion(
            transform.position, puntos, quienDisparo?.imPlayer1 ?? true, efecto);
        Destroy(gameObject);
    }

    void Romper()
    {
        GameObject fracturado = Instantiate(prefabFracturado, transform.position, transform.rotation);
        Rigidbody[] trozos = fracturado.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in trozos)
        {
            rb.AddForce(Random.insideUnitSphere * 3f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
        Destroy(fracturado, 5f);
    }

    private void OnDestroy()
    {
        if (spawnPuerta != null)
            spawnPuerta.CerrarPuerta();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Botella"))
            Explosion();
    }

    void Explosion()
    {
        GameObject fracturado = Instantiate(prefabFracturado, transform.position, transform.rotation);
        Rigidbody[] trozos = fracturado.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in trozos)
        {
            rb.AddForce(Random.insideUnitSphere * 3f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
        Destroy(gameObject);
        Destroy(fracturado, 5f);
    }
}