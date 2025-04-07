using UnityEngine;
using UnityEngine.InputSystem;

public class ControlJugador : MonoBehaviour
{
    public float throwForce = 1f;
    public float jumpForce = 80f;
    public float customGravity = -40f;  // Gravedad personalizada para el cubo

    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector2 moveInput;

    private bool isGrounded = true;
    private int jumpCount = 0;  // Contador de saltos

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Desactivar la gravedad predeterminada del Rigidbody
        rb.useGravity = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isGrounded || jumpCount < 2)  // Permitir saltar si está en el suelo o si tiene saltos restantes
            {
                rb.AddForce(Vector3.up * jumpForce * 10, ForceMode.Impulse);
                jumpCount++;  // Incrementar el contador de saltos
            }
            else
            {
                Debug.Log("Max jump count reached.");
            }
        }
    }

    private void Update()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 direction = camForward * moveInput.y + camRight * moveInput.x;

            rb.AddForce(direction * throwForce, ForceMode.Impulse);
        }

        // Limitar velocidad máxima para evitar que se mueva demasiado rápido
        float maxSpeed = 5f;
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // Detección precisa si el cubo está tocando el suelo usando Raycast
    private bool IsGrounded()
    {
        float distance = 0.6f;  // Ajusta esto si es necesario
        return Physics.Raycast(transform.position, Vector3.down, distance);
    }

    // Método para restablecer el contador de saltos cuando el cubo toque el suelo
    private void OnCollisionEnter(Collision collision)
    {
        if (IsGrounded())  // Restablecer el contador al tocar el suelo
        {
            jumpCount = 0;  // Reiniciar el contador de saltos
            Debug.Log("Jump count reset.");
        }
    }

    private void FixedUpdate()
    {
        // Asegurarse de actualizar la variable isGrounded de manera continua
        isGrounded = IsGrounded();

        // Si el cubo no está en el suelo, aplicar la gravedad personalizada
        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);
        }
    }
}
