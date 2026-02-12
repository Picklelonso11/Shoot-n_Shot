using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class RarityItem
    {
        public GameObject prefab; // Prefab de la botella
        public int puntos;        // 10, 25, 50 o 75
        public int weight;        // Probabilidad
    }

    [Header("Botellas")]
    public RarityItem[] items;

    [Header("Spawns")]
    public SpawnPoint[] spawnPoints;

    [Header("Ronda")]
    public RondaManager rondaManager;

    private GameObject botellaActual;

    public AudioSource sonidoPuerta;
    public AudioSource sonidoRejilla;

    // Temporizador entre spawns
    private float spawnTimer = 0f;
    private float spawnInterval = 1.5f;

    // Guarda el último spawn usado para NO repetirlo
    private SpawnPoint ultimoSpawn;

    void Update()
    {
        if (!rondaManager.RondaEnCurso())
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        // Si se destruyó la botella antes de 2s: spawn inmediato y reiniciar contador
        if (botellaActual == null)
        {
            SpawnBottle();
            spawnTimer = 0f;
            return;
        }

        // Si pasan 2 segundos y la botella sigue,forzar nuevo spawn
        if (spawnTimer >= spawnInterval)
        {
            SpawnBottle();
            spawnTimer = 0f;
        }
    }

    // ================= SPAWN PRINCIPAL =================
    void SpawnBottle()
    {
        // Elegir spawn distinto al anterior
        SpawnPoint sp = GetWeightedSpawnPointDifferent();

        bool esRejilla = sp.CompareTag("Rejilla");
        bool esPuerta = sp.CompareTag("Puerta");

        // Elegir botella según el spawnPoint
        RarityItem itemSeleccionado = (esRejilla || esPuerta) ? GetItem50o75() : GetWeightedItem();

        // Instanciar botella
        botellaActual = Instantiate(itemSeleccionado.prefab, sp.transform.position, Quaternion.identity);

        // Guardar último spawn usado
        ultimoSpawn = sp;

        // Sonidos 
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

        // Abrir puerta
        SpawnPuerta spawnPuerta = sp.GetComponent<SpawnPuerta>();
        if (spawnPuerta != null)
        {
            spawnPuerta.AbrirPuerta();

            Objetivo obj = botellaActual.GetComponent<Objetivo>();
            if (obj != null)
            {
                obj.spawnPuerta = spawnPuerta;
            }
        }

        // Movimiento de la botella
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
                mov.SetDirection(haciaDerecha);
            }
        }

        // Rotación para rejilla
        if (esRejilla)
        {
            botellaActual.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }

    // ========== SPAWN SIN REPETIR ==========
    SpawnPoint GetWeightedSpawnPointDifferent()
    {
        List<SpawnPoint> candidatos = new List<SpawnPoint>();

        foreach (var sp in spawnPoints)
        {
            // Evita repetir el mismo spawn 
            if (sp != ultimoSpawn)
                candidatos.Add(sp);
        }

        // Si solo hay uno disponible
        if (candidatos.Count == 0)
            return spawnPoints[0];

        // Calcular peso total
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

    // ========== BOTELLA NORMAL POR PESO ==========
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

    // ========== SOLO BOTELLAS DE 50 O 75 ==========
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

            // Reinicia el contador para spawn inmediato
            spawnTimer = spawnInterval;
        }
    }
}
