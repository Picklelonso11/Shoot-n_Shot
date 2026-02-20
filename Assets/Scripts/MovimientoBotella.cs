using UnityEngine;
using System.Collections;

public class MovimientoBotella : MonoBehaviour
{
    [Header("Movimiento ronda 1")]
    public float speed = 3f;
    public float amplitude = 1f;
    public float frequency = 2f;

    [Header("Ronda 2")]
    public float fuerzaVertical = 4.5f;
    public float velocidadRotacion = 360f;

    [Header("Gravedad dinámica ronda 2")]
    public float gravedadSubiendo = 3f;
    public float gravedadBajando = 12f;
    public float multiplicadorTiempo = 1.5f;

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
                transform.Rotate(Vector3.forward * velocidadRotacion * Time.deltaTime);
                break;

            case TipoMovimiento.SinMovimiento:
                break;
        }
    }

    void FixedUpdate()
    {
        if (tipoMovimiento != TipoMovimiento.RotarYSaltar || rb == null || rb.isKinematic)
            return;

        Vector3 v = rb.linearVelocity;

        bool subiendo = v.y > 0f;
        float gravedad = subiendo ? gravedadSubiendo : gravedadBajando;

        // Aplicamos gravedad manual y desactivamos la de Unity
        rb.linearVelocity += Vector3.down * gravedad * multiplicadorTiempo * Time.fixedDeltaTime;
    }

    public void SetDirection(bool haciaDerecha)
    {
        moveDir = haciaDerecha ? Vector3.right : Vector3.left;
    }

    // ================= CONFIGURACIÓN POR RONDA =================
    public void ConfigurarPorRonda(int ronda, bool spawnLateral)
    {
        rb = GetComponent<Rigidbody>();

        if (ronda == 1)
        {
            tipoMovimiento = TipoMovimiento.Normal;
            return;
        }

        if (ronda == 2)
        {
            tipoMovimiento = TipoMovimiento.RotarYSaltar;

            if (rb == null) return;

            // Activar físicas 
            rb.isKinematic = false;
            rb.useGravity = false;          // Usamos gravedad manual
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.linearDamping = 1.2f;
            rb.angularDamping = 0.4f;

            if (spawnLateral)
            {
                Vector3 impulso = moveDir + Vector3.up * 0.6f;
                rb.AddForce(impulso.normalized * fuerzaVertical, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(Vector3.up * fuerzaVertical, ForceMode.Impulse);
            }

            return;
        }

        if (ronda == 3)
        {
            tipoMovimiento = TipoMovimiento.SinMovimiento;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    // ================= MOVIMIENTO RONDA 1 =================
    void MovimientoNormal()
    {
        time += Time.deltaTime;

        float y = startY + Mathf.Sin(time * frequency) * amplitude;

        transform.position += moveDir * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
