using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class VacaSeguidora : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 3.5f;
    public float distanciaParaFrenar = 1.5f;

    [Header("Audio")]
    public AudioClip[] sonidosBalidos;
    public float tiempoMinimoEntreSonidos = 4f;
    public float tiempoMaximoEntreSonidos = 8f;

    private Transform prota;
    private Rigidbody rb;
    private AudioSource audioSource;
    private float timer;

    void Start()
    {
        // Buscamos al jugador por Tag
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null) prota = jugador.transform;

        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        timer = Random.Range(tiempoMinimoEntreSonidos, tiempoMaximoEntreSonidos);
    }

    void FixedUpdate()
    {
        if (prota != null)
        {
            float distancia = Vector3.Distance(transform.position, prota.position);

            if (distancia > distanciaParaFrenar)
            {
                Vector3 direccion = (prota.position - transform.position).normalized;
                rb.linearVelocity = new Vector3(direccion.x * velocidad, rb.linearVelocity.y, direccion.z * velocidad);

                direccion.y = 0;
                if (direccion != Vector3.zero)
                {
                    transform.forward = direccion;
                }
            }
            else
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }

    void Update()
    {
        // Lógica de sonido aleatorio
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
            audioSource.pitch = Random.Range(0.85f, 1.15f); // Variación de tono natural
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
                Debug.LogWarning("No se encontró el GameManager en la escena para restar los puntos.");
            }
            Destroy(gameObject);
        }
    }
}