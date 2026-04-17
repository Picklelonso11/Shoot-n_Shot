using Unity.Jobs;
using UnityEngine;
using System.Collections;
using System.Linq;

public class MoverChupito : MonoBehaviour
{
    public enum TipoObjeto { TipoJ1, TipoJ2 }

    [System.Serializable]
    public class Item
    {
        public Transform objeto;
        public Transform destino;
        [HideInInspector] public bool completado = false;
        [HideInInspector] public Vector3 posicionInicial;
    }

    int numeroChupitosJ1 = 0;
    int numeroChupitosJ2 = 0;

    public Item[] tipoJ1;
    public Item[] tipoJ2;

    public float velocidad = 3f;

    public AudioSource slideChupito;
    public AudioSource chupitoConseguido;

    private Coroutine movimientoActual;

    public MirillaMovement mirillaPlayer1;
    public MirillaMovement mirillaPlayer2;

    private void Start()
    {
        GuardarPosiciones(tipoJ1);
        GuardarPosiciones(tipoJ2);
    }

    // ── Añadir chupito ────────────────────────────────────────────────────────
    public void MoverSiguiente(TipoObjeto tipo)
    {
        if (movimientoActual != null) return;

        Item[] lista = tipo == TipoObjeto.TipoJ1 ? tipoJ1 : tipoJ2;

        if (tipo == TipoObjeto.TipoJ1 && numeroChupitosJ1 < 5)
        {
            numeroChupitosJ1++;
            mirillaPlayer1.Borrachera(numeroChupitosJ1);
        }
        else if (tipo == TipoObjeto.TipoJ2 && numeroChupitosJ2 < 5)
        {
            numeroChupitosJ2++;
            mirillaPlayer2.Borrachera(numeroChupitosJ2);
        }

        foreach (Item item in lista)
        {
            if (!item.completado)
            {
                movimientoActual = StartCoroutine(MoverObjeto(item, haciaPosicion: true));
                break;
            }
        }
    }

    // ── Restar chupito ────────────────────────────────────────────────────────
    public void RestarChupito(TipoObjeto tipo)
    {
        if (movimientoActual != null) return;

        Item[] lista = tipo == TipoObjeto.TipoJ1 ? tipoJ1 : tipoJ2;

        if (tipo == TipoObjeto.TipoJ1 && numeroChupitosJ1 > 0)
        {
            // Encontrar el último chupito completado y devolverlo
            for (int i = lista.Length - 1; i >= 0; i--)
            {
                if (lista[i].completado)
                {
                    numeroChupitosJ1--;
                    mirillaPlayer1.Borrachera(numeroChupitosJ1);
                    ScoreManager.Instance.AddScore(player1: true, puntos: -150);
                    movimientoActual = StartCoroutine(MoverObjeto(lista[i], haciaPosicion: false));
                    break;
                }
            }
        }
        else if (tipo == TipoObjeto.TipoJ2 && numeroChupitosJ2 > 0)
        {
            for (int i = lista.Length - 1; i >= 0; i--)
            {
                if (lista[i].completado)
                {
                    numeroChupitosJ2--;
                    mirillaPlayer2.Borrachera(numeroChupitosJ2);
                    ScoreManager.Instance.AddScore(player1: false, puntos: -150);
                    movimientoActual = StartCoroutine(MoverObjeto(lista[i], haciaPosicion: false));
                    break;
                }
            }
        }
    }

    // ── Mover objeto (ida o vuelta) ───────────────────────────────────────────
    private IEnumerator MoverObjeto(Item item, bool haciaPosicion)
    {
        Vector3 destino = haciaPosicion ? item.destino.position : item.posicionInicial;

        while (Vector3.Distance(item.objeto.position, destino) > 0.05f)
        {
            Vector3 dir = (destino - item.objeto.position).normalized;
            item.objeto.position += dir * velocidad * Time.deltaTime;
            yield return null;
        }

        item.objeto.position = destino;
        item.completado = haciaPosicion; // true si fue hacia destino, false si volvió
        movimientoActual = null;

        slideChupito.Play();
        while (slideChupito.isPlaying) yield return null;
        chupitoConseguido.Play();
    }

    void GuardarPosiciones(Item[] lista)
    {
        foreach (Item item in lista)
            if (item.objeto != null)
                item.posicionInicial = item.objeto.position;
    }

    public void ResetearChupitos()
    {
        ResetearLista(tipoJ1);
        ResetearLista(tipoJ2);
        mirillaPlayer1.Borrachera(0);
        mirillaPlayer2.Borrachera(0);
        numeroChupitosJ1 = 0;
        numeroChupitosJ2 = 0;
    }

    void ResetearLista(Item[] lista)
    {
        foreach (Item item in lista)
        {
            if (item.objeto != null)
            {
                item.objeto.position = item.posicionInicial;
                item.completado = false;
            }
        }
    }
}