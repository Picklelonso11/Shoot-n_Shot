using Unity.Jobs;
using UnityEngine;
using System.Collections;

public class MoverChupito : MonoBehaviour
{
    // Tipo de grupo de objetos
    public enum TipoObjeto
    {
        TipoJ1,
        TipoJ2
    }

    [System.Serializable]
    public class Item
    {
        public Transform objeto;
        public Transform destino;
        [HideInInspector] public bool completado = false;
    }

    // Dos listas, una por cada tipo de objetos
    public Item[] tipoJ1;
    public Item[] tipoJ2;

    public float velocidad = 3f;

    public AudioSource slideChupito;
    public AudioSource chupitoConseguido;

    private Coroutine movimientoActual;

    // Inicia el movimiento de un tipo específico
    public void MoverSiguiente(TipoObjeto tipo)
    {
        if (movimientoActual != null)
        {
            return; // ya hay un objeto moviéndose
        }

        Item[] lista = tipo == TipoObjeto.TipoJ1 ? tipoJ1 : tipoJ2;

        // Busca el primer objeto que aún no se haya movido
        foreach (Item item in lista)
        {
            if (!item.completado)
            {
                movimientoActual = StartCoroutine(MoverObjeto(item));
                break;
            }
        }
    }
    private IEnumerator MoverObjeto(Item item)
    {
        while (Vector3.Distance(item.objeto.position, item.destino.position) > 0.05f)
        {
            Vector3 dir = (item.destino.position - item.objeto.position).normalized;
            item.objeto.position += dir * velocidad * Time.deltaTime;
            yield return null;
        }

        // Colocar exactamente en destino y marcar completado
        item.objeto.position = item.destino.position;
        item.completado = true;

        movimientoActual = null;

        // Reproducir el primer sonido
        slideChupito.Play();

        // Esperar a que termine el primero
        while (slideChupito.isPlaying)
        {
            yield return null;
        }

        // Reproducir el segundo sonido
        chupitoConseguido.Play();
    }
}
