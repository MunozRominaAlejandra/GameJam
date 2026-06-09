using UnityEngine;

public class GeneradorVaca : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabVaca;
    public Transform puntoAparicion; // Usando tu nombre original

    [Header("Control de Generación")]
    public int limiteVacas = 1; // Podés cambiar este número en el Inspector
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
            // Crea la vaca en el punto de aparición
            Instantiate(prefabVaca, puntoAparicion.position, puntoAparicion.rotation);

            // Suma 1 al contador para que no genere infinitas
            vacasGeneradas++;
        }
        else
        {
            Debug.LogWarning("Falta asignar el Prefab Vaca o el Punto Aparicion en el Inspector.");
        }
    }
}