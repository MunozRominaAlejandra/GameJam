using UnityEngine;

public class CamaraIsometricaPura : MonoBehaviour
{
    [Header("Referencias")]
    public Transform prota; // Arrastrá a tu jugador acá

    [Header("Configuración de Zoom")]
    public Vector3 baseOffset = new Vector3(-30f, 40f, -30f); // Distancia Age of Empires
    public Vector3 zoomStepPerCow = new Vector3(-2f, 3f, -2f); // Cuánto se aleja por vaca
    public float smoothSpeed = 3f;

    private Vector3 targetOffset;
    private int currentCows = 0;

    void Start()
    {
        targetOffset = baseOffset;

        // Clavamos la rotación isométrica perfecta al iniciar
        transform.rotation = Quaternion.Euler(45f, 45f, 0f);

        // Ajuste del FOV para el efecto de maqueta
        Camera miCamara = GetComponent<Camera>();
        if (miCamara != null)
        {
            miCamara.fieldOfView = 30f;
        }
    }

    // Usamos LateUpdate para la cámara, así aseguramos que el jugador ya se movió este frame
    void LateUpdate()
    {
        if (prota == null) return;

        // Calculamos dónde debería estar la cámara sumando la posición del prota + la distancia
        Vector3 posicionDeseada = prota.position + targetOffset;

        // Movemos la cámara suavemente hacia esa posición
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, Time.deltaTime * smoothSpeed);
    }

    // ==========================================
    // Funciones para llamar desde tus otros scripts
    // ==========================================
    public void AddCow()
    {
        currentCows++;
        UpdateTargetOffset();
    }

    public void RemoveCow()
    {
        if (currentCows > 0)
        {
            currentCows--;
            UpdateTargetOffset();
        }
    }

    private void UpdateTargetOffset()
    {
        targetOffset = baseOffset + (zoomStepPerCow * currentCows);
    }
}