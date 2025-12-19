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
    public RarityItem[] items;          // Todas las botellas posibles

    [Header("Spawns")]
    public SpawnPoint[] spawnPoints;     // TODOS los puntos de spawn

    [Header("Ronda")]
    public RondaManager rondaManager;

    private GameObject botellaActual;   // Referencia a la botella viva
    public AudioSource sonidoPuerta;   
    public AudioSource sonidoRejilla; 


    void Update()
    {
        // Solo spawnea si la ronda está activa y no hay botella
        if (rondaManager.RondaEnCurso() && botellaActual == null)
        {
            SpawnBottle();
        }
    }

    // SPAWN PRINCIPAL
    void SpawnBottle()
    {
        // Elegir spawn según peso
        SpawnPoint sp = GetWeightedSpawnPoint();

        bool esRejilla = sp.CompareTag("Rejilla");
        bool esPuerta = sp.CompareTag("Puerta");

        RarityItem itemSeleccionado;

        // Si es Rejilla o Puerta solo botellas de 50 o 75 puntos
        if (esRejilla || esPuerta)
        {
            itemSeleccionado = GetItem50o75();
        }
        else
        {
            // Spawn normal 
            itemSeleccionado = GetWeightedItem();
        }

        // Instanciar botella y comprobar si se instanció dentro de una puerta
        botellaActual = Instantiate(itemSeleccionado.prefab, sp.transform.position, Quaternion.identity);

        //aplicamos los sonidos de puerta o de reilla
        if (esRejilla)
        {
            sonidoRejilla.Play();
        }
        if (esPuerta)
        {
            sonidoPuerta.Play();
        }

        SpawnPuerta spawnPuerta = sp.GetComponent<SpawnPuerta>();

        if (spawnPuerta != null)
        {
            spawnPuerta.AbrirPuerta();

            // Pasamos la referencia del SpawnPoint a la botella
            Objetivo obj = botellaActual.GetComponent<Objetivo>();
            if (obj != null)
            {
                obj.spawnPuerta = spawnPuerta;
            }
        }

        // CONFIGURACIÓN DE MOVIMIENTO
        MovimientoBotella mov = botellaActual.GetComponent<MovimientoBotella>();

        if (mov != null)
        {
            // Si es rejilla o puerta no se mueve
            if (esRejilla || esPuerta)
            {
                mov.enabled = false;
            }
            else
            {
                // Movimiento normal según dirección del spawner
                bool haciaDerecha = sp.CompareTag("Derecha");
                mov.SetDirection(haciaDerecha);
            }
        }

        // ROTACIÓN PARA REJILLA 
        if (esRejilla)
        {
            botellaActual.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }

    // SELECCIÓN DE SPAWN POR PESO
    SpawnPoint GetWeightedSpawnPoint()
    {
        int totalWeight = 0;

        foreach (var sp in spawnPoints)
        {
            totalWeight += sp.weight;
        }

        int r = Random.Range(0, totalWeight);
        int sum = 0;

        foreach (var sp in spawnPoints)
        {
            sum += sp.weight;
            if (r < sum)
                return sp;
        }

        return spawnPoints[0];
    }

    // SELECCIÓN NORMAL POR PESO
    RarityItem GetWeightedItem()
    {
        int totalWeight = 0;
        foreach (var item in items)
        {
            totalWeight += item.weight;
        }

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

    // SOLO BOTELLAS DE 50 O 75 PUNTOS
    RarityItem GetItem50o75()
    {
        // Filtrar botellas válidas
        var validas = new System.Collections.Generic.List<RarityItem>();

        foreach (var item in items)
        {
            if (item.puntos == 50 || item.puntos == 75)
            {
                validas.Add(item);
            }
        }

        // Elegir una al azar
        return validas[Random.Range(0, validas.Count)];
    }

    public void DestruirBotellaActual()
    {
        // Si hay una botella viva en escena
        if (botellaActual != null)
        {
            Destroy(botellaActual);
            botellaActual = null;
        }
    }
}
