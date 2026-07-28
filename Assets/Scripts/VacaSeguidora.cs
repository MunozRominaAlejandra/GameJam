using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class VacaSeguidora : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float velocidad = 3.5f;
    public float distanciaParaFrenar = 1.5f;
    public float velocidadRotacion = 200f;

    [Header("Audio")]
    public AudioClip[] sonidosBalidos;
    public float tiempoMinimoEntreSonidos = 4f;
    public float tiempoMaximoEntreSonidos = 8f;

    private Transform prota;
    private Rigidbody rb;
    private AudioSource audioSource;
    private float timer;

    // Variable para optimizacion matematica
    private float distanciaParaFrenarSq;

    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null) prota = jugador.transform;

        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        timer = Random.Range(tiempoMinimoEntreSonidos, tiempoMaximoEntreSonidos);

        // VARIACION ORGANICA: Asignamos una distancia de frenado unica para cada vaca.
        // Esto evita que todas intenten ocupar la misma coordenada exacta y colapsen las fisicas.
        distanciaParaFrenar = Random.Range(1.8f, 3.2f);

        // 1. OPTIMIZACION MATEMATICA EN INICIALIZACION:
        // Elevamos la distancia al cuadrado una sola vez al inicio para evitar calcular 
        // raices cuadradas (Mathf.Sqrt) iterativas en el FixedUpdate.
        distanciaParaFrenarSq = distanciaParaFrenar * distanciaParaFrenar;
    }

    void FixedUpdate()
    {
        if (prota != null)
        {
            // 2. ALGEBRA VECTORIAL: Calculo de vector resultante (A -> B = B - A)
            Vector3 offset = prota.position - transform.position;

            // Restringimos la evaluacion geometrica al plano horizontal XZ
            offset.y = 0;

            // 3. JUSTIFICACION TEORICA (Magnitud al Cuadrado):
            // Para optimizar el rendimiento y evitar el calculo costoso de raices cuadradas 
            // en el ciclo de fisicas, se evalua la magnitud al cuadrado de la distancia 
            // (dx^2 + dz^2) frente a la variable de frenado previamente elevada (Freno^2).
            if (offset.sqrMagnitude > distanciaParaFrenarSq)
            {
                // Normalizamos el vector para obtener solo la direccion (magnitud de 1)
                Vector3 direccion = offset.normalized;

                // Mantenemos la traslacion fisica
                rb.linearVelocity = new Vector3(direccion.x * velocidad, rb.linearVelocity.y, direccion.z * velocidad);

                // 4. JUSTIFICACION TEORICA (Producto Vectorial / Cross Product):
                // Se reemplaza el calculo trigonometrico directo por el producto vectorial
                // entre el vector direccional Forward de la vaca y el vector hacia el objetivo.
                // Vector_C = Adelante x Direccion.
                Vector3 cross = Vector3.Cross(transform.forward, direccion);

                // El componente Y del vector ortogonal resultante determina geometricamente 
                // el sentido del giro (-1 a 1). Si Y es positivo, el objetivo esta a la derecha. 
                // Si es negativo, a la izquierda. Esto optimiza la rotacion computacional.
                float sentidoGiro = cross.y;

                // Aplicamos la rotacion manualmente usando el sentido matematico
                transform.Rotate(0, sentidoGiro * velocidadRotacion * Time.fixedDeltaTime, 0);
            }
            else
            {
                // Si la vaca esta dentro del radio de frenado, anulamos la velocidad horizontal
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ReproducirSonido();
            timer = Random.Range(tiempoMinimoEntreSonidos, tiempoMaximoEntreSonidos);
        }
    }

    void ReproducirSonido()
    {
        if (sonidosBalidos.Length > 0 && !audioSource.isPlaying)
        {
            audioSource.clip = sonidosBalidos[Random.Range(0, sonidosBalidos.Length)];
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.Play();
        }
    }

    private void OnCollisionEnter(Collision colision)
    {
        if (colision.gameObject.CompareTag("Enemigo"))
        {
            Debug.Log("¡Vaca robada!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RemoveCow();
            }
            else
            {
                Debug.LogWarning("No se encontro el GameManager en la escena para restar los puntos.");
            }
            Destroy(gameObject);
        }
    }
}