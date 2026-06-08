using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class IsometricRunner : MonoBehaviour
{
    [Header("Configuración de Movimiento (Sin Arcos)")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 1200f; // Rotación súper rápida para que gire en el lugar

    private CharacterController controller;
    private Animator animator;

    // Esta variable guarda la dirección real de movimiento, independiente de hacia dónde mire el modelo 3D
    private Vector3 currentMoveDirection;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Inicia corriendo hacia arriba-derecha
        currentMoveDirection = new Vector3(1, 0, 1).normalized;
    }

    private void Update()
    {
        HandleMovementAndRotation();
        UpdateAnimations();
    }

    private void HandleMovementAndRotation()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        // 1. EL SECRETO: Cambiamos la dirección de movimiento de forma INSTANTÁNEA al tocar la tecla
        if (inputDir.magnitude >= 0.1f)
        {
            currentMoveDirection = Quaternion.Euler(0, 45, 0) * inputDir;
        }

        // 2. MOVIMIENTO FÍSICO: Nos movemos usando la dirección instantánea, NO el 'transform.forward'
        // Esto evita totalmente que el personaje trace curvas o se vaya a otra posición.
        Vector3 velocity = currentMoveDirection * moveSpeed;
        velocity.y = -9.81f; // Gravedad

        controller.Move(velocity * Time.deltaTime);

        // 3. ROTACIÓN VISUAL: Hacemos que el cuerpo del personaje gire sobre su propio eje para alcanzar la dirección de movimiento
        if (currentMoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMoveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", moveSpeed);
            animator.SetFloat("MotionSpeed", 1f);
            animator.SetBool("Grounded", true);
        }
    }
}