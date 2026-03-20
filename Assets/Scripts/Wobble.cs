using UnityEngine;

public class Wobble : MonoBehaviour
{
    MeshRenderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    // Use this for initialization
    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        if (rend == null)
        {
            Debug.LogError("No rend found in: " + name);
        }
        velocity = Vector3.zero;
        lastPos = Vector3.zero;
        lastRot = Vector3.zero;
        angularVelocity = Vector3.zero;

    }
    private void Update()
    {
        //// decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX,0, Mathf.Clamp(Time.deltaTime * (Recovery), 0f, 1f));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ,0, Mathf.Clamp(Time.deltaTime * (Recovery), 0f, 1f));
        //Debug.Log("WX: " + wobbleAmountToAddX);
        // make a sine wave of the decreasing wobble
        time += Time.deltaTime;
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);
        // send it to the shader
        rend.material.SetFloat("_WobbleZ", wobbleAmountX);
        rend.material.SetFloat("_WobbleX", wobbleAmountZ);
        // velocity
        if (Time.deltaTime != 0)
        {
            velocity = (lastPos - transform.position) / Time.deltaTime;
        }
        angularVelocity = transform.rotation.eulerAngles - lastRot;
        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }
}