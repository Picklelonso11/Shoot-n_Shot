using UnityEngine;

public class MovimientoBotella : MonoBehaviour
{
    public float speed = 3f;
    public float amplitude = 1f;
    public float frequency = 2f;

    private float startY;
    private float time;

    private Vector3 moveDir; // dirección de movimiento

    public void SetDirection(bool haciaDerecha)
    {
        moveDir = haciaDerecha ? Vector3.right : Vector3.left;
    }

    void Start()
    {
        startY = transform.position.y;

        // Seguridad por si no llaman SetDirection
        if (moveDir == Vector3.zero)
        {
            moveDir = Vector3.right;
        }
    }

    void Update()
    {
        time += Time.deltaTime;

        float y = startY + Mathf.Sin(time * frequency) * amplitude;

        transform.position += moveDir * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
