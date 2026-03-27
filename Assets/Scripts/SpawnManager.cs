using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class RarityItem
    {
        public GameObject prefab;
        public int puntos;
        public EfectoBotella efecto = EfectoBotella.Normal;
        public int weight;
    }

    [Header("Botellas")]
    public RarityItem[] items;

    [Header("Spawns Ronda 1")]
    public SpawnPoint[] spawnRonda1;
    [Header("Spawns Ronda 2")]
    public SpawnPoint[] spawnRonda2;
    [Header("Spawns Ronda 3")]
    public SpawnPoint[] spawnRonda3;

    [Header("Ronda")]
    public RondaManager rondaManager;

    [Header("Referencias jugadores")]
    public MirillaMovement mirillaJ1;
    public MirillaMovement mirillaJ2;
    public Ammunition ammoJ1;
    public Ammunition ammoJ2;
    public MoverChupito moverChupito;

    public AudioSource sonidoPuerta;
    public AudioSource sonidoRejilla;

    private float spawnTimer = 0f;
    private float spawnInterval = 1f;
    private SpawnPoint ultimoSpawn;

    void Update()
    {
        if (!rondaManager.RondaEnCurso()) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnBottle();
            spawnTimer = 0f;
        }
    }

    void SpawnBottle()
    {
        SpawnPoint[] spawnsActuales = GetSpawnsDeRonda();
        SpawnPoint sp = GetWeightedSpawnPointDifferent(spawnsActuales);

        bool esRejilla = sp.CompareTag("Rejilla");
        bool esPuerta = sp.CompareTag("Puerta");
        bool esLanzado = sp.gameObject.layer == LayerMask.NameToLayer("Throws");
        int ronda = rondaManager.RondaActual();
        bool esLateral = sp.CompareTag("Izquierda") || sp.CompareTag("Derecha");

        RarityItem itemSeleccionado;

        if (esRejilla || esPuerta)
            itemSeleccionado = GetItemEspecial();
        else if (ronda == 1 && esLanzado)
            itemSeleccionado = GetItemEspecial();
        else if (ronda == 2 && esLateral)
            itemSeleccionado = GetItemEspecial();
        else
            itemSeleccionado = GetWeightedItem();

        GameObject nuevaBotella = Instantiate(
            itemSeleccionado.prefab, sp.transform.position, Quaternion.identity);

        ultimaBotella = nuevaBotella;
        ultimoSpawn = sp;

        // ── Asignar datos al Objetivo ─────────────────────────────────────────
        Objetivo obj = nuevaBotella.GetComponent<Objetivo>();
        if (obj != null)
        {
            obj.puntos = itemSeleccionado.puntos;
            obj.efecto = itemSeleccionado.efecto;
            obj.mirillaJ1 = mirillaJ1;
            obj.mirillaJ2 = mirillaJ2;
            obj.ammoJ1 = ammoJ1;
            obj.ammoJ2 = ammoJ2;
            obj.moverChupito = moverChupito;
        }

        // ── Sonidos ───────────────────────────────────────────────────────────
        if (esRejilla) sonidoRejilla.Play();
        if (esPuerta) sonidoPuerta.Play();

        // ── Puerta ────────────────────────────────────────────────────────────
        SpawnPuerta spawnPuerta = sp.GetComponent<SpawnPuerta>();
        if (spawnPuerta != null)
        {
            spawnPuerta.AbrirPuerta();
            if (obj != null) obj.spawnPuerta = spawnPuerta;
        }

        // ── Movimiento ────────────────────────────────────────────────────────
        MovimientoBotella mov = nuevaBotella.GetComponent<MovimientoBotella>();
        if (mov != null)
        {
            if (esRejilla || esPuerta)
                mov.enabled = false;
            else
            {
                mov.SetDirection(sp.CompareTag("Derecha"));
                mov.ConfigurarPorRonda(ronda, esLateral, esLanzado);
            }
        }

        // ── Rotación rejilla ──────────────────────────────────────────────────
        if (esRejilla)
            nuevaBotella.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        // ── Tiempo de vida ────────────────────────────────────────────────────
        float tiempoVida = (esRejilla || esPuerta) ? 2f : 4f;
        Destroy(nuevaBotella, tiempoVida);
    }

    SpawnPoint[] GetSpawnsDeRonda()
    {
        switch (rondaManager.RondaActual())
        {
            case 1: return spawnRonda1;
            case 2: return spawnRonda2;
            case 3: return spawnRonda3;
            default: return spawnRonda1;
        }
    }

    SpawnPoint GetWeightedSpawnPointDifferent(SpawnPoint[] lista)
    {
        List<SpawnPoint> candidatos = new List<SpawnPoint>();
        foreach (var sp in lista)
            if (sp != ultimoSpawn) candidatos.Add(sp);
        if (candidatos.Count == 0) return lista[0];

        int totalWeight = 0;
        foreach (var sp in candidatos) totalWeight += sp.weight;
        int r = Random.Range(0, totalWeight), sum = 0;
        foreach (var sp in candidatos) { sum += sp.weight; if (r < sum) return sp; }
        return candidatos[0];
    }

    // Todas las botellas por peso
    RarityItem GetWeightedItem()
    {
        int totalWeight = 0;
        foreach (var item in items) totalWeight += item.weight;
        int r = Random.Range(0, totalWeight), sum = 0;
        foreach (var item in items) { sum += item.weight; if (r < sum) return item; }
        return items[0];
    }

    // Solo botellas de efecto especial (BorrachoRival, RestarChupito, VaciarMuni y sus contrarios)
    RarityItem GetItemEspecial()
    {
        var validas = new List<RarityItem>();
        foreach (var item in items)
            if (item.efecto != EfectoBotella.Normal)
                validas.Add(item);

        if (validas.Count == 0) return GetWeightedItem();

        int totalWeight = 0;
        foreach (var item in validas) totalWeight += item.weight;
        int r = Random.Range(0, totalWeight), sum = 0;
        foreach (var item in validas) { sum += item.weight; if (r < sum) return item; }
        return validas[0];
    }

    // Referencia a la última botella spawneada para destrucción manual
    private GameObject ultimaBotella;

    public void DestruirBotellaActual()
    {
        if (ultimaBotella != null)
        {
            Destroy(ultimaBotella);
            ultimaBotella = null;
        }
        spawnTimer = spawnInterval;
    }
}