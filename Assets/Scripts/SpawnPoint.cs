using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Peso de aparición")]
    public int weight = 10; 

    [HideInInspector] public bool ocupado = false;
}
