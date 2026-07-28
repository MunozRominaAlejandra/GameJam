using UnityEngine;
using System.Collections;

public class EfectoCrecimiento : MonoBehaviour
{
    [Header("Ajustes de Transformacion")]
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

            // 1. ALGEBRA MATRICIAL (Interpolacion Lineal Manual):
            // Calculamos los factores de escala independientes para cada eje cartesiano
            // en funcion del avance temporal normalizado (0.0 a 1.0).
            float sx = tamañoFinal.x * porcentaje;
            float sy = tamañoFinal.y * porcentaje;
            float sz = tamañoFinal.z * porcentaje;

            // 2. CONSTRUCCION DE LA MATRIZ HOMOGENEA DE ESCALA 4x4:
            // Construimos una matriz diagonal S. Los factores de transformacion (sx, sy, sz)
            // ocupan la diagonal principal.
            Matrix4x4 matrizEscala = new Matrix4x4();
            matrizEscala.SetRow(0, new Vector4(sx, 0, 0, 0));
            matrizEscala.SetRow(1, new Vector4(0, sy, 0, 0));
            matrizEscala.SetRow(2, new Vector4(0, 0, sz, 0));

            // JUSTIFICACION TEORICA (Coordenada W):
            // El 1 final en la posicion [3,3] es obligatorio para preservar la 
            // coordenada homogenea intacta, evitando el colapso del espacio 3D.
            matrizEscala.SetRow(3, new Vector4(0, 0, 0, 1));

            // 3. TRANSFORMACION POR PRODUCTO MATRIZ-VECTOR (S * v):
            // Multiplicamos la matriz de 4x4 por un vector columna unitario base [1,1,1,1].
            // El resultado proyecta la escala volumetrica frame a frame.
            Vector4 vectorBase = new Vector4(1f, 1f, 1f, 1f);
            Vector4 escalaResultante = matrizEscala * vectorBase;

            try
            {
                // Extraemos los componentes cartesianos (x, y, z), descartando la dimension W
                transform.localScale = new Vector3(escalaResultante.x, escalaResultante.y, escalaResultante.z);
            }
            catch (MissingReferenceException)
            {
                yield break; // Nos vamos en silencio si la entidad fue destruida por un gaucho
            }

            yield return null;
        }

        // 4. VALIDACION DE CIERRE (Clamping con Matriz Absoluta):
        // Para corregir posibles imprecisiones de punto flotante derivadas del Time.deltaTime,
        // se construye una matriz final absoluta que clava las proporciones geometricas al 100%.
        if (this != null && gameObject != null)
        {
            try
            {
                Matrix4x4 matrizFinal = new Matrix4x4();
                matrizFinal.SetRow(0, new Vector4(tamañoFinal.x, 0, 0, 0));
                matrizFinal.SetRow(1, new Vector4(0, tamañoFinal.y, 0, 0));
                matrizFinal.SetRow(2, new Vector4(0, 0, tamañoFinal.z, 0));
                matrizFinal.SetRow(3, new Vector4(0, 0, 0, 1));

                Vector4 escalaTransformada = matrizFinal * new Vector4(1f, 1f, 1f, 1f);
                transform.localScale = new Vector3(escalaTransformada.x, escalaTransformada.y, escalaTransformada.z);
            }
            catch (MissingReferenceException) { }
        }
    }
}