using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class Disparo : MonoBehaviour
{
    public event Action haDisparado; 

    [HideInInspector] public bool imPlayer1;
    public RawImage mirilla;
    public RawImage chispas;
    public Camera camara;
    public GameObject [] balazosPrefabs;
    public GunFollow gunFollow;

    [Header("Decals")]
    public int maxDecals = 10; // Límite de decals en escena
    private Queue<GameObject> decalPool = new Queue<GameObject>(); // Cola de decals activos

    private Gamepad myGamepad;

    public AudioSource sonidoAcierto;   // Sonido de botella rota
    public AudioSource sonidoFallo;     // Sonido de fallo
    public AudioSource sonidoBoton;
    Ammunition ammunition;
    

    void Start()
    {
        ammunition = GetComponent<Ammunition>();
        // Ocultar las chispas del disparo
        chispas.enabled = false;
        // Identificar jugador
        imPlayer1 = gameObject.CompareTag("Player1");

        // Asignar mando según tag
        if (Gamepad.all.Count >= 1 && imPlayer1)
        {
            myGamepad = Gamepad.all[0]; // Mando J1
        }
        if (Gamepad.all.Count >= 2 && !imPlayer1)
        {
            myGamepad = Gamepad.all[1]; // Mando J2
        }
    }
    void Update()
    {
        DisparoJugador(imPlayer1);
    }
    private void SpawnDecal(Vector3 point, Vector3 normal)
    {
        // Si se alcanza el límite, destruye el decal más antiguo
        if (decalPool.Count >= maxDecals)
        {
            GameObject oldest = decalPool.Dequeue();
            Destroy(oldest);
        }

        int randomIndex = UnityEngine.Random.Range(0, balazosPrefabs.Length);
        GameObject prefabSeleccionado = balazosPrefabs[randomIndex];

        GameObject marca = Instantiate(
            prefabSeleccionado,
            point + normal * 0.001f,
            Quaternion.LookRotation(normal));

        marca.transform.Rotate(0f, -180f, 0f);

        decalPool.Enqueue(marca); // Añade el nuevo decal a la cola
    }
    private void AspectoMirilla()
    {
        RectTransform rt = mirilla.rectTransform;   
        RawImage img = mirilla;                     

        Vector3 originalScale = rt.localScale;

        Sequence seq = DOTween.Sequence();

        // Primera mitad: más pequeña + más oscura
        seq.Append(rt.DOScale(1.5f, 0.05f).SetEase(Ease.OutQuad));
        seq.Join(img.DOColor(new Color(0.35f, 0.35f, 0.35f, 1f), 0.05f));

        // Segunda mitad: vuelve a la escala y al color original
        seq.Append(rt.DOScale(originalScale, 0.05f).SetEase(Ease.InQuad));
        seq.Join(img.DOColor(Color.white, 0.05f));
    }
    private void DisparoJugador(bool player1)
    {
        bool disparo = false;

        // Comprobar input primero
        if (myGamepad != null &&
            (myGamepad.rightShoulder.wasPressedThisFrame || myGamepad.leftShoulder.wasPressedThisFrame))
        {
            disparo = true;
        }
        if (!disparo && player1 && Keyboard.current.leftCtrlKey.wasPressedThisFrame)
            disparo = true;
        if (!disparo && !player1 && Keyboard.current.rightCtrlKey.wasPressedThisFrame)
            disparo = true;

        // Solo disparar bala si hay input Y hay balas disponibles
        if (!disparo || !ammunition.TryShoot()) return;

        gunFollow?.Retroceso();
        haDisparado?.Invoke();
        AspectoMirilla();

        if (!disparo)
        {
            return;
        }
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, mirilla.rectTransform.position);


        // RAYCAST UI
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            // Comprobar que es layer UI
            if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                sonidoBoton.Play();
                Button btn = result.gameObject.GetComponent<Button>();

                if (btn != null)
                {
                    btn.onClick.Invoke(); // Ejecuta el botón
                    return;
                }
            }
        }


        // RAYCAST 3D
        Ray ray = camara.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            StartCoroutine(Flash());
            if (hit.collider.CompareTag("Botella"))
            {
                Objetivo target = hit.collider.GetComponent<Objetivo>();
                target.Disparado(this);
                sonidoAcierto.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                sonidoAcierto.Play();
                sonidoFallo.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                sonidoFallo.Play();
            }
            else
            {
                Debug.Log("Disparo fallido");
                sonidoFallo.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                sonidoFallo.Play();
                if (hit.collider.CompareTag("Mueble"))
                {
                    SpawnDecal(hit.point, hit.normal); 
                    Debug.Log("MUEBLE");
                }
            }
        }
        else
        {
            StartCoroutine(Flash());
            Debug.Log("Disparo fallido");
            sonidoFallo.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
            sonidoFallo.Play(); 
        }

    }
    private IEnumerator Flash()
    {
        chispas.rectTransform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
        chispas.enabled = true;      // aparece
        yield return new WaitForSeconds(0.05f);
        chispas.enabled = false;     // desaparece
    }
}
