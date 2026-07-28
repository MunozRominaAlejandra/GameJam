using UnityEngine;

public class GeneradorVaca : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabVaca;
    public Transform puntoAparicion;

    [Header("Control de Generación")]
    public int limiteVacas = 1; // Cantidad máxima de vacas que puede entregar este spawnpoint
    private int vacasGeneradas = 0;

    private void OnTriggerEnter(Collider otro)
    {
        // Verifica si el que atraviesa la caja es el jugador
        if (otro.CompareTag("Player"))
        {
            // Verifica si todavía no alcanzó el límite configurado
            if (vacasGeneradas < limiteVacas)
            {
                GenerarVaca();
            }
        }
    }

    private void GenerarVaca()
    {
        if (prefabVaca != null && puntoAparicion != null)
        {
            // 1. Crea la vaca en el punto de aparición
            Instantiate(prefabVaca, puntoAparicion.position, puntoAparicion.rotation);

            // 2. Suma 1 al contador local para no generar infinitas
            vacasGeneradas++;

            // 3. Le avisa al GameManager para incrementar la manada y el multiplicador de puntos
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCow();
            }
            else
            {
                Debug.LogWarning("No se encontró el GameManager en la escena para sumar los puntos.");
            }
        }
        else
        {
            Debug.LogWarning("Falta asignar el Prefab Vaca o el Punto Aparicion en el Inspector.");
        }
    }
}