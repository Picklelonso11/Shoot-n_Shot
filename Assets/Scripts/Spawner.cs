using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    // Clase que define un ítem con su rareza y peso de aparición
    [System.Serializable]
    public class RarityItem
    {
        public GameObject prefab; // Prefab del objeto a instanciar (por ejemplo, una botella)
        public string tipo;       // Tipo de rareza: "comun", "raro", "epico", "legendario"
        public int weight;        // Peso o probabilidad (más alto = más frecuente)
    }

    // Lista de ítems con sus pesos configurados desde el Inspector
    public RarityItem[] items;

    // Tiempo entre apariciones (por defecto, cada 2 segundos)
    public float spawnInterval = 2f;

    // Duración total del periodo de aparición (por ejemplo, 30 segundos)
    public float spawnDuration = 30f;

    void Start()
    {
        // Inicia la rutina de aparición al comenzar el juego
        StartCoroutine(SpawnRoutine());
    }

    // Corrutina que controla el proceso de aparición durante un tiempo limitado
    IEnumerator SpawnRoutine()
    {
        float timePassed = 0f; // Variable para llevar el control del tiempo transcurrido

        // Mientras no haya pasado el tiempo total configurado (spawnDuration)
        while (timePassed < spawnDuration)
        {
            // Llama a la función que genera un objeto según la probabilidad de rareza
            SpawnOne();

            // Espera el tiempo indicado antes de generar el siguiente
            yield return new WaitForSeconds(spawnInterval);

            // Suma el tiempo transcurrido al contador
            timePassed += spawnInterval;
        }

        // Cuando termina el ciclo de spawn, se puede hacer algo opcional (como terminar ronda)
    }

    // Instancia un objeto de acuerdo a su rareza y lo configura para que se mueva
    void SpawnOne()
    {
        // Obtiene un prefab según la probabilidad de rareza (más peso = más frecuencia)
        RarityItem itemSeleccionado = GetWeightedItem();

        // Instancia el prefab en la posición del spawner con rotación neutra
        GameObject obj = Instantiate(itemSeleccionado.prefab, transform.position, Quaternion.identity);

        // Asigna el tipo de botella en el componente Objetivo para que sume puntos correctamente
        Objetivo objetivo = obj.GetComponent<Objetivo>();
        if (objetivo != null)
        {
            // El tipo se establece automáticamente según la rareza configurada
            objetivo.tipoBotella = itemSeleccionado.tipo;
        }

        // Comprueba si el objeto tiene un componente de movimiento
        MovimientoBotella mov = obj.GetComponent<MovimientoBotella>();
        if (mov != null)
        {
            // Determina la dirección del movimiento según el tag del spawner:
            // Si el Spawner tiene tag "Derecha", se moverán hacia la derecha; si no, hacia la izquierda
            bool haciaDerecha = CompareTag("Derecha");

            // Llama al método del script MovimientoBotella para establecer la dirección
            mov.SetDirection(haciaDerecha);
        }
    }

    // Selecciona un ítem de la lista según su peso de aparición
    RarityItem GetWeightedItem()
    {
        // Calcula la suma total de todos los pesos (ejemplo: 70 + 20 + 8 + 2 = 100)
        int totalWeight = 0;
        foreach (var item in items)
            totalWeight += item.weight;

        // Genera un número aleatorio entre 0 y el total de pesos
        int r = Random.Range(0, totalWeight);

        // Recorre la lista acumulando pesos hasta encontrar el rango correspondiente
        int sum = 0;
        foreach (var item in items)
        {
            sum += item.weight;

            // Cuando el número aleatorio cae dentro del rango actual, se elige este ítem
            if (r < sum)
                return item;
        }

        // Si por alguna razón no entra en ningún rango (muy raro), devuelve el primero como respaldo
        return items[0];
    }
}
