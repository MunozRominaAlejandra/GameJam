using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class EnemigoInteligente : MonoBehaviour
{
    [Header("Configuracion General")]
    public float velocidadVagar = 1.5f;
    public float velocidadPersecucion = 4.0f;
    public float radioDeteccion = 10f;
    public float radioVagar = 5f;

    [Header("Vision y Radar")]
    [Tooltip("1 = Vision laser, 0.2 = Cono muy amplio (150 grados), 0.5 = Cono estandar (120 grados)")]
    [Range(-1f, 1f)]
    public float umbralVision = 0.2f;

    [Tooltip("Si el jugador entra en este radio, el enemigo lo detecta automaticamente por la espalda o costados.")]
    public float radioRadarCercano = 2.5f;

    [Header("Audio")]
    public AudioClip[] sonidosDeteccion;

    [Header("Animacion")]
    public Animator animator;
    public string nombreParametroVelocidad = "Speed";

    private Transform prota;
    private Vector3 posicionInicial;
    private bool estaPersiguiendo = false;
    private NavMeshAgent agente;
    private AudioSource audioSource;

    // Variables para almacenar las distancias al cuadrado y optimizar calculos matematicos
    private float radioDeteccionSq;
    private float radioRadarCercanoSq;

    void Start()
    {
        try
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null) prota = jugador.transform;

            posicionInicial = transform.position;
            agente = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();

            // 1. OPTIMIZACION MATEMATICA EN INICIALIZACION:
            // Elevamos los radios al cuadrado una sola vez al inicio. 
            // Esto prepara las variables para usar sqrMagnitude en el Update, 
            // evitando calcular costosas raices cuadradas (Mathf.Sqrt) en cada frame.
            radioDeteccionSq = radioDeteccion * radioDeteccion;
            radioRadarCercanoSq = radioRadarCercano * radioRadarCercano;

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

            // Calculamos el vector direccional (A -> B) restando las posiciones
            Vector3 vectorHaciaProta = prota.position - transform.position;

            // Obtenemos la suma de los catetos al cuadrado (dx^2 + dz^2)
            float distanciaCuadrada = vectorHaciaProta.sqrMagnitude;

            // 2. FILTRO DE RENDIMIENTO (Magnitud al Cuadrado):
            // Comparamos los cuadrados de las distancias (d^2 <= r^2)
            if (distanciaCuadrada <= radioDeteccionSq)
            {
                if (estaPersiguiendo)
                {
                    ContinuarPersecucion();
                }
                else
                {
                    // Condicion A: Radar de proximidad extrema (deteccion omnidireccional)
                    bool estaMuyCerca = (distanciaCuadrada <= radioRadarCercanoSq);

                    // 3. ANALISIS DE SISTEMA FORMAL (Producto Escalar / Dot Product):
                    // Normalizamos el vector de distancia para que su longitud sea 1.
                    // Al aplicar Vector3.Dot entre el frente del gaucho y la direccion del jugador,
                    // el algebra vectorial nos devuelve exactamente el COSENO del angulo formado.
                    // Si este coseno supera el umbralVision, el jugador esta dentro del cono visual.
                    Vector3 direccionHaciaProta = vectorHaciaProta.normalized;
                    float productoPunto = Vector3.Dot(transform.forward, direccionHaciaProta);
                    bool vistoDeFrente = (productoPunto > umbralVision);

                    // Transicion de estado: Si se cumple cualquier condicion, inicia el "Chase"
                    if (estaMuyCerca || vistoDeFrente)
                    {
                        IniciarPersecucion();
                    }
                }
            }
            else
            {
                // Si el jugador se alejo del radio de deteccion, la IA transiciona a patrullaje
                PerderInteres();
            }

            // --- VAGAR AUTOMATICO ---
            if (!estaPersiguiendo && agente.isOnNavMesh && !agente.pathPending && agente.remainingDistance < 0.5f)
            {
                ElegirNuevoPuntoVagar();
            }

            // --- ANIMACIONES ---
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

    void IniciarPersecucion()
    {
        estaPersiguiendo = true;

        if (sonidosDeteccion != null && sonidosDeteccion.Length > 0 && audioSource != null)
        {
            int indiceAleatorio = Random.Range(0, sonidosDeteccion.Length);
            if (sonidosDeteccion[indiceAleatorio] != null)
            {
                audioSource.PlayOneShot(sonidosDeteccion[indiceAleatorio]);
            }
        }

        ContinuarPersecucion();
    }

    void ContinuarPersecucion()
    {
        if (agente.isOnNavMesh)
        {
            agente.speed = velocidadPersecucion;
            agente.SetDestination(prota.position);
        }
    }

    void PerderInteres()
    {
        if (estaPersiguiendo)
        {
            estaPersiguiendo = false;
            if (agente.isOnNavMesh)
            {
                agente.speed = velocidadVagar;
                ElegirNuevoPuntoVagar();
            }
        }
    }

    void ElegirNuevoPuntoVagar()
    {
        try
        {
            if (agente == null) return;

            // 4. LOGICA ESPACIAL DE PATRULLAJE:
            // Generamos un punto aleatorio en un circulo 2D imaginario y lo escalamos al radio de patrullaje.
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioVagar;
            Vector3 destino = posicionInicial + new Vector3(puntoAleatorio.x, 0, puntoAleatorio.y);

            // Verificamos que el punto matematico calculado caiga efectivamente dentro del area navegable (NavMesh)
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