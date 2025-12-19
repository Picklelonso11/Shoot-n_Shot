using UnityEngine;

public class LímiteBotella : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Botella"))
        {
            Destroy(other.gameObject);
        }
    }
}
