using UnityEngine;
using System.Collections;

public class EfectoCrecimiento : MonoBehaviour
{
    [Header("Ajustes de Transformación")]
    public float tiempoCrecimiento = 1.5f;
    public Vector3 tamañoFinal = new Vector3(1f, 1f, 1f);

    private void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(CrecerPocoAPoco());
    }

    private IEnumerator CrecerPocoAPoco()
    {
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoCrecimiento)
        {
            // Doble check de seguridad
            if (this == null || gameObject == null) yield break;

            tiempoPasado += Time.deltaTime;
            float porcentaje = tiempoPasado / tiempoCrecimiento;

            // TRY-CATCH: Si el personaje elimina la vaca en este milisegundo exacto, 
            // atrapamos el error silenciosamente y abortamos.
            try
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, tamañoFinal, porcentaje);
            }
            catch (MissingReferenceException)
            {
                yield break; // Nos vamos en silencio
            }

            yield return null;
        }

        // Validación final antes de clavar el tamaño máximo
        if (this != null && gameObject != null)
        {
            try
            {
                transform.localScale = tamañoFinal;
            }
            catch (MissingReferenceException) { }
        }
    }
}