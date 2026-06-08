using UnityEngine;

public class GeneradorVaca : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabVaca; // Arrastrá acá tu Cubo_Vaca convertido en Prefab
    public Transform puntoAparicion; // Un GameObject vacío donde querés que spawnee (ej. un metro fuera de la casa)
    private bool yaVaciado = false; // Para que no spawnee vacas infinitas

    private void OnTriggerEnter(Collider otro)
    {
        // Si entra el prota y todavía no robamos esta casa
        if (otro.CompareTag("Player") && !yaVaciado)
        {
            Instantiate(prefabVaca, puntoAparicion.position, Quaternion.identity);
            yaVaciado = true;
            Debug.Log("¡Vaca liberada!");
        }
    }
}