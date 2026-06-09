using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class EnemigoInteligente : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadVagar = 1.5f;
    public float velocidadPersecucion = 4.0f;
    public float radioDeteccion = 7f;
    public float radioVagar = 5f;

    [Header("Audio")]
    [Tooltip("Arrastrá acá tus 3 audios. El sistema elegirá uno al azar.")]
    public AudioClip[] sonidosDeteccion; // Ahora es un arreglo para guardar múltiples audios

    [Header("Animación")]
    public Animator animator;
    public string nombreParametroVelocidad = "Speed";

    private Transform prota;
    private Vector3 posicionInicial;
    private bool estaPersiguiendo = false;
    private NavMeshAgent agente;
    private AudioSource audioSource;

    void Start()
    {
        try
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null) prota = jugador.transform;

            posicionInicial = transform.position;
            agente = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();

            if (agente != null)
            {
                agente.speed = velocidadVagar;
                ElegirNuevoPuntoVagar();
            }
        }
        catch (MissingReferenceException)
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        try
        {
            if (prota == null || agente == null) return;

            float distanciaAlProta = Vector3.Distance(transform.position, prota.position);

            if (distanciaAlProta <= radioDeteccion)
            {
                if (!estaPersiguiendo)
                {
                    estaPersiguiendo = true;

                    // --- NUEVA LÓGICA DE AUDIO ALEATORIO ---
                    if (sonidosDeteccion != null && sonidosDeteccion.Length > 0 && audioSource != null)
                    {
                        // Elegimos un número al azar entre 0 y la cantidad total de audios
                        int indiceAleatorio = Random.Range(0, sonidosDeteccion.Length);

                        // Verificamos que ese espacio no esté vacío y reproducimos
                        if (sonidosDeteccion[indiceAleatorio] != null)
                        {
                            audioSource.PlayOneShot(sonidosDeteccion[indiceAleatorio]);
                        }
                    }
                }

                if (agente.isOnNavMesh)
                {
                    agente.speed = velocidadPersecucion;
                    agente.SetDestination(prota.position);
                }
            }
            else
            {
                estaPersiguiendo = false;
                agente.speed = velocidadVagar;

                if (agente.isOnNavMesh && !agente.pathPending && agente.remainingDistance < 0.5f)
                {
                    ElegirNuevoPuntoVagar();
                }
            }

            // --- EL PUENTE HACIA LA ANIMACIÓN ---
            if (animator != null && agente.isOnNavMesh)
            {
                animator.SetFloat(nombreParametroVelocidad, agente.velocity.magnitude);
                animator.SetBool("Grounded", true);
                animator.SetFloat("MotionSpeed", 1f);
            }
        }
        catch (MissingReferenceException)
        {
            this.enabled = false;
        }
    }

    void ElegirNuevoPuntoVagar()
    {
        try
        {
            if (agente == null) return;

            Vector2 puntoAleatorio = Random.insideUnitCircle * radioVagar;
            Vector3 destino = posicionInicial + new Vector3(puntoAleatorio.x, 0, puntoAleatorio.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(destino, out hit, 5.0f, NavMesh.AllAreas))
            {
                if (agente.isOnNavMesh) agente.SetDestination(hit.position);
            }
        }
        catch (MissingReferenceException)
        {
            this.enabled = false;
        }
    }
}