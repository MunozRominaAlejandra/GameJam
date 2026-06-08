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
    public AudioClip sonidoDeteccion;

    private Transform prota;
    private Vector3 posicionInicial;
    private bool estaPersiguiendo = false;
    private NavMeshAgent agente;
    private AudioSource audioSource;

    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null) prota = jugador.transform;

        posicionInicial = transform.position;
        agente = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        agente.speed = velocidadVagar;
        ElegirNuevoPuntoVagar();
    }

    void Update()
    {
        if (prota == null) return;

        float distanciaAlProta = Vector3.Distance(transform.position, prota.position);

        if (distanciaAlProta <= radioDeteccion)
        {
            // Entró en radio de detección
            if (!estaPersiguiendo)
            {
                estaPersiguiendo = true;
                if (sonidoDeteccion != null) audioSource.PlayOneShot(sonidoDeteccion);
            }

            agente.speed = velocidadPersecucion;
            agente.SetDestination(prota.position);
        }
        else
        {
            // Está patrullando
            estaPersiguiendo = false;
            agente.speed = velocidadVagar;

            if (!agente.pathPending && agente.remainingDistance < 0.5f)
            {
                ElegirNuevoPuntoVagar();
            }
        }
    }

    void ElegirNuevoPuntoVagar()
    {
        Vector2 puntoAleatorio = Random.insideUnitCircle * radioVagar;
        Vector3 destino = posicionInicial + new Vector3(puntoAleatorio.x, 0, puntoAleatorio.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destino, out hit, 5.0f, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
        }
    }
}