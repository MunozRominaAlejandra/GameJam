using UnityEngine;
using Unity.Cinemachine; // Súper importante: Este es el nuevo namespace para Cinemachine 3 en Unity 6

public class IsometricCameraManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CinemachineCamera cinemachineCam;

    [Header("Configuración de Zoom")]
    [SerializeField] private Vector3 baseOffset = new Vector3(0, 10f, -10f); // Tu distancia inicial
    [SerializeField] private Vector3 zoomStepPerCow = new Vector3(0, 1.5f, -1.5f); // Cuánto sube (Y) y retrocede (Z) por cada vaca
    [SerializeField] private float smoothSpeed = 3f; // Velocidad de la transición de la cámara

    // Componente interno de Cinemachine que maneja la distancia
    private CinemachineFollow followComponent;

    // Offset al que la cámara intentará llegar suavemente
    private Vector3 targetOffset;

    // Contador interno (solo para la cámara)
    private int currentCows = 0;

    private void Start()
    {
        // Obtenemos el componente que controla la posición
        if (cinemachineCam != null)
        {
            followComponent = cinemachineCam.GetComponent<CinemachineFollow>();
        }

        targetOffset = baseOffset;

        // Aplicamos la posición inicial
        if (followComponent != null)
        {
            followComponent.FollowOffset = targetOffset;
        }
    }

    private void Update()
    {
        // En cada frame, acercamos el offset actual al targetOffset de forma suave
        if (followComponent != null && followComponent.FollowOffset != targetOffset)
        {
            followComponent.FollowOffset = Vector3.Lerp(
                followComponent.FollowOffset,
                targetOffset,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    // ====================================================================
    // FUNCIONES PÚBLICAS (Para llamarlas desde tu script de colisiones)
    // ====================================================================

    /// <summary>
    /// Llama a esta función cuando el personaje choque con una vaca.
    /// </summary>
    public void AddCow()
    {
        currentCows++;
        UpdateTargetOffset();
    }

    /// <summary>
    /// Llama a esta función si los gauchos le quitan vacas al jugador.
    /// </summary>
    public void RemoveCow()
    {
        if (currentCows > 0)
        {
            currentCows--;
            UpdateTargetOffset();
        }
    }

    // Calcula a qué distancia debería estar la cámara basándose en las vacas actuales
    private void UpdateTargetOffset()
    {
        // Suma el paso de zoom multiplicado por la cantidad de vacas
        targetOffset = baseOffset + (zoomStepPerCow * currentCows);
    }
}