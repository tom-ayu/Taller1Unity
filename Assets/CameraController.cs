using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 2f;
    public float followDistance = 5f;
    public Vector3 offset = new Vector3(0f, 2f, 0f);

    private Vector2 lookInput;
    private float currentYaw = 0f;
    private float currentPitch = 20f;
    private const float pitchMin = -30f;
    private const float pitchMax = 70f;

    private float noInputTimer = 0f;
    public float resetDelay = 5f;

    public float defaultYaw = 0f;
    public float defaultPitch = 20f;
    public Vector3 defaultOffset = new Vector3(0f, 2f, 0f);

    public float resetLerpSpeed = 2f;

    private bool isResetting = false;

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (target == null) return;

        if (lookInput.magnitude > 0.1f)
        {
            currentYaw += lookInput.x * rotationSpeed;
            currentPitch -= lookInput.y * rotationSpeed;
            currentPitch = Mathf.Clamp(currentPitch, pitchMin, pitchMax);
            noInputTimer = 0f;
            isResetting = false; // Cancelar reset si hay input
        }
        else
        {
            noInputTimer += Time.deltaTime;

            if (noInputTimer >= resetDelay)
            {
                isResetting = true;
            }
        }

        if (isResetting)
        {
            // Interpolamos gradualmente pitch y yaw hacia los valores por defecto
            currentYaw = Mathf.Lerp(currentYaw, defaultYaw, Time.deltaTime * resetLerpSpeed);
            currentPitch = Mathf.Lerp(currentPitch, defaultPitch, Time.deltaTime * resetLerpSpeed);
            offset = Vector3.Lerp(offset, defaultOffset, Time.deltaTime * resetLerpSpeed);

            // Cancelar reset cuando esté cerca del objetivo
            if (Mathf.Abs(currentYaw - defaultYaw) < 0.1f &&
                Mathf.Abs(currentPitch - defaultPitch) < 0.1f &&
                Vector3.Distance(offset, defaultOffset) < 0.05f)
            {
                isResetting = false;
            }
        }

        // Calcular rotación y posición
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 position = target.position - rotation * Vector3.forward * followDistance + offset;

        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * resetLerpSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), Time.deltaTime * resetLerpSpeed);
    }
}
