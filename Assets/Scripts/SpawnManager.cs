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
        public int weight;
    }

    [Header("Botellas")]
    public RarityItem[] items;

    // ===== SPAWNS POR RONDA =====
    [Header("Spawns Ronda 1")]
    public SpawnPoint[] spawnRonda1;

    [Header("Spawns Ronda 2")]
    public SpawnPoint[] spawnRonda2;

    [Header("Spawns Ronda 3")]
    public SpawnPoint[] spawnRonda3;

    [Header("Ronda")]
    public RondaManager rondaManager;

    private GameObject botellaActual;

    public AudioSource sonidoPuerta;
    public AudioSource sonidoRejilla;

    private float spawnTimer = 0f;
    private float spawnInterval = 1.5f;

    private SpawnPoint ultimoSpawn;

    void Update()
    {
        if (!rondaManager.RondaEnCurso())
            return;

        spawnTimer += Time.deltaTime;

        if (botellaActual == null)
        {
            SpawnBottle();
            spawnTimer = 0f;
            return;
        }

        if (spawnTimer >= spawnInterval)
        {
            SpawnBottle();
            spawnTimer = 0f;
        }
    }

    // ================= SPAWN PRINCIPAL =================
    void SpawnBottle()
    {
        SpawnPoint[] spawnsActuales = GetSpawnsDeRonda();

        SpawnPoint sp = GetWeightedSpawnPointDifferent(spawnsActuales);

        bool esRejilla = sp.CompareTag("Rejilla");
        bool esPuerta = sp.CompareTag("Puerta");

        RarityItem itemSeleccionado =
            (esRejilla || esPuerta) ? GetItem50o75() : GetWeightedItem();

        botellaActual = Instantiate(itemSeleccionado.prefab, sp.transform.position, Quaternion.identity);

        ultimoSpawn = sp;

        if (esRejilla)
        {
            sonidoRejilla.Play();
            Destroy(botellaActual, 1.8f);
        }

        if (esPuerta)
        {
            sonidoPuerta.Play();
            Destroy(botellaActual, 1.8f);
        }

        SpawnPuerta spawnPuerta = sp.GetComponent<SpawnPuerta>();
        if (spawnPuerta != null)
        {
            spawnPuerta.AbrirPuerta();

            Objetivo obj = botellaActual.GetComponent<Objetivo>();
            if (obj != null)
                obj.spawnPuerta = spawnPuerta;
        }

        MovimientoBotella mov = botellaActual.GetComponent<MovimientoBotella>();
        if (mov != null)
        {
            if (esRejilla || esPuerta)
            {
                mov.enabled = false;
            }
            else
            {
                bool haciaDerecha = sp.CompareTag("Derecha");
                bool spawnLateral = sp.CompareTag("Derecha") || sp.CompareTag("Izquierda");

                mov.SetDirection(haciaDerecha);
                mov.ConfigurarPorRonda(rondaManager.RondaActual(), spawnLateral);
            }
        }


        if (esRejilla)
            botellaActual.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    // ===== OBTENER SPAWNS SEGÚN RONDA =====
    SpawnPoint[] GetSpawnsDeRonda()
    {
        int ronda = rondaManager.RondaActual();

        switch (ronda)
        {
            case 1: return spawnRonda1;
            case 2: return spawnRonda2;
            case 3: return spawnRonda3;
            default: return spawnRonda1;
        }
    }

    // ========== SPAWN SIN REPETIR ==========
    SpawnPoint GetWeightedSpawnPointDifferent(SpawnPoint[] lista)
    {
        List<SpawnPoint> candidatos = new List<SpawnPoint>();

        foreach (var sp in lista)
            if (sp != ultimoSpawn)
                candidatos.Add(sp);

        if (candidatos.Count == 0)
            return lista[0];

        int totalWeight = 0;
        foreach (var sp in candidatos)
            totalWeight += sp.weight;

        int r = Random.Range(0, totalWeight);
        int sum = 0;

        foreach (var sp in candidatos)
        {
            sum += sp.weight;
            if (r < sum)
                return sp;
        }

        return candidatos[0];
    }

    // ========== BOTELLA NORMAL ==========
    RarityItem GetWeightedItem()
    {
        int totalWeight = 0;
        foreach (var item in items)
            totalWeight += item.weight;

        int r = Random.Range(0, totalWeight);
        int sum = 0;

        foreach (var item in items)
        {
            sum += item.weight;
            if (r < sum)
                return item;
        }

        return items[0];
    }

    // ========== SOLO 50 O 75 ==========
    RarityItem GetItem50o75()
    {
        var validas = new List<RarityItem>();

        foreach (var item in items)
            if (item.puntos == 50 || item.puntos == 75)
                validas.Add(item);

        return validas[Random.Range(0, validas.Count)];
    }

    // ========== DESTRUCCIÓN MANUAL ==========
    public void DestruirBotellaActual()
    {
        if (botellaActual != null)
        {
            Destroy(botellaActual);
            botellaActual = null;
            spawnTimer = spawnInterval;
        }
    }
}
