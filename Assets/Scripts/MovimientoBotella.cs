using UnityEngine;

public class MovimientoBotella : MonoBehaviour
{
    public float speed = 3f;
    public float amplitude = 1f;
    public float frequency = 2f;

    [Header("Ronda 2")]
    public float fuerzaVertical = 3f;
    public float velocidadRotacion = 360f;

    private float startY;
    private float time;
    private Vector3 moveDir;

    private enum TipoMovimiento
    {
        Normal,
        RotarYSaltar,
        SinMovimiento
    }

    private TipoMovimiento tipoMovimiento = TipoMovimiento.Normal;

    private Rigidbody rb;

    public void SetDirection(bool haciaDerecha)
    {
        moveDir = haciaDerecha ? Vector3.right : Vector3.left;
    }

    // configurar desde SpawnManager
    public void ConfigurarPorRonda(int ronda, bool spawnLateral)
    {
        rb = GetComponent<Rigidbody>();

        if (ronda == 1)
        {
            tipoMovimiento = TipoMovimiento.Normal;
        }
        else if (ronda == 2)
        {
            if (spawnLateral)
            {
                tipoMovimiento = TipoMovimiento.Normal;
            }
            else
            {
                tipoMovimiento = TipoMovimiento.RotarYSaltar;

                if (rb != null)
                {
                    rb.isKinematic = false;     // permitir física
                    rb.useGravity = true;       // que caiga

                    rb.linearVelocity = Vector3.zero; // limpiar velocidad previa
                    rb.AddForce(Vector3.up * fuerzaVertical, ForceMode.Impulse);
                }

            }
        }
        else if (ronda == 3)
        {
            tipoMovimiento = TipoMovimiento.SinMovimiento;
        }
    }

    void Start()
    {
        startY = transform.position.y;

        if (moveDir == Vector3.zero)
            moveDir = Vector3.right;
    }

    void Update()
    {
        switch (tipoMovimiento)
        {
            case TipoMovimiento.Normal:
                MovimientoNormal();
                break;

            case TipoMovimiento.RotarYSaltar:
                transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
                break;

            case TipoMovimiento.SinMovimiento:
                break;
        }
    }

    void MovimientoNormal()
    {
        time += Time.deltaTime;

        float y = startY + Mathf.Sin(time * frequency) * amplitude;

        transform.position += moveDir * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
