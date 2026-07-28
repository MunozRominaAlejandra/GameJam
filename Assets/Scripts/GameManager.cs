using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Sistema de Puntuación Progresiva")]
    public int score;
    private float puntosAcumulados = 0f;

    [Tooltip("Puntos base por segundo que genera cada vaca.")]
    public float puntosBasePorSegundo = 5f;

    [Tooltip("Multiplicador acumulativo por cada vaca extra (+0.5 = +50% de ritmo).")]
    public float multiplicadorExtraPorVaca = 0.5f;

    [Header("Conteo de Vacas")]
    public int cows = 0;
    private bool yaTuvoVacas = false;

    [Header("Paneles de UI")]
    public GameObject panelPausa;
    public GameObject panelGameOver;

    [Header("Game Over & Meme")]
    public VideoPlayer videoPlayerMeme;
    public TMP_Text textoPuntajes;
    public TMP_Text textoPuntos;

    [Header("Música de Fondo (Playlist)")]
    public AudioSource reproductorMusica;
    public AudioClip[] pistasMusicales; // Arrastrá acá la Pista 1 y la Pista 2
    [Range(0f, 1f)] public float volumenDuranteJuego = 0.3f; // Volumen normal bajito
    [Range(0f, 1f)] public float volumenEnPausa = 0.8f;      // Volumen alto para cuando se pausa
    private int pistaActual = 0;

    private bool juegoPausado = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        score = 0;
        puntosAcumulados = 0f;

        if (cows > 0)
        {
            yaTuvoVacas = true;
        }

        ActualizarUI();
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        Time.timeScale = 1f;

        // --- INICIAR MÚSICA DE FONDO ---
        if (reproductorMusica != null && pistasMusicales.Length > 0)
        {
            reproductorMusica.volume = volumenDuranteJuego;
            reproductorMusica.clip = pistasMusicales[0];
            reproductorMusica.loop = false; // Lo apagamos para que pase a la siguiente canción al terminar
            reproductorMusica.Play();
        }
    }

    private void Update()
    {
        // Generación de puntos progresiva si hay al menos una vaca activa
        if (cows > 0 && Time.timeScale > 0f)
        {
            float multiplicadorActual = 1f + ((cows - 1) * multiplicadorExtraPorVaca);
            float puntosGanados = cows * puntosBasePorSegundo * multiplicadorActual * Time.deltaTime;

            puntosAcumulados += puntosGanados;
            score = Mathf.FloorToInt(puntosAcumulados);

            ActualizarUI();
        }

        // --- GESTOR DE PLAYLIST AUTOMÁTICO ---
        // Si la música terminó de sonar, no estamos en pausa ni en Game Over, pasamos a la siguiente pista
        if (reproductorMusica != null && pistasMusicales.Length > 0 && !reproductorMusica.isPlaying && !juegoPausado && (panelGameOver == null || !panelGameOver.activeSelf))
        {
            pistaActual++;
            // Si llegamos al final de la lista, volvemos a la pista 0
            if (pistaActual >= pistasMusicales.Length) pistaActual = 0;

            reproductorMusica.clip = pistasMusicales[pistaActual];
            reproductorMusica.Play();
        }

        // Pausa con la tecla Escape O con la tecla P
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && (panelGameOver == null || !panelGameOver.activeSelf))
        {
            if (juegoPausado) ReanudarJuego();
            else PausarJuego();
        }
    }

    // --- LÓGICA DE SPAWN Y PÉRDIDA DE VACAS ---
    public void AddCow()
    {
        cows++;
        yaTuvoVacas = true;
        ActualizarUI();
    }

    public void RemoveCow()
    {
        if (cows > 0)
        {
            cows--;
        }

        ActualizarUI();

        if (cows <= 0 && yaTuvoVacas)
        {
            cows = 0;
            GameOver();
        }
    }

    private void ActualizarUI()
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = "Puntos: " + score;
        }
    }

    // --- PAUSA Y NAVEGACIÓN ---
    public void PausarJuego()
    {
        juegoPausado = true;
        Time.timeScale = 0f;
        if (panelPausa != null) panelPausa.SetActive(true);

        // INTENSIFICAR MÚSICA
        if (reproductorMusica != null) reproductorMusica.volume = volumenEnPausa;
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        Time.timeScale = 1f;
        if (panelPausa != null) panelPausa.SetActive(false);

        // NORMALIZAR MÚSICA
        if (reproductorMusica != null) reproductorMusica.volume = volumenDuranteJuego;
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RegresarAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menus");
    }

    // --- GAME OVER Y TOP 3 ---
    public void GameOver()
    {
        Time.timeScale = 0f;
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(true);
        if (videoPlayerMeme != null) videoPlayerMeme.Play();

        // APAGAR LA MÚSICA DE FONDO PARA DEJAR SONAR EL MEME
        if (reproductorMusica != null) reproductorMusica.Stop();

        GestionarTop3();
    }

    private void GestionarTop3()
    {
        int top1 = PlayerPrefs.GetInt("Top1", 0);
        int top2 = PlayerPrefs.GetInt("Top2", 0);
        int top3 = PlayerPrefs.GetInt("Top3", 0);

        if (score > top1)
        {
            top3 = top2;
            top2 = top1;
            top1 = score;
        }
        else if (score > top2)
        {
            top3 = top2;
            top2 = score;
        }
        else if (score > top3)
        {
            top3 = score;
        }

        PlayerPrefs.SetInt("Top1", top1);
        PlayerPrefs.SetInt("Top2", top2);
        PlayerPrefs.SetInt("Top3", top3);
        PlayerPrefs.Save();

        if (textoPuntajes != null)
        {
            textoPuntajes.text =
                "¡TE QUEDASTE SIN VACAS!\n\n" +
                "PUNTAJE FINAL: " + score + "\n\n" +
                "TOP 3 HISTÓRICO:\n" +
                "1. " + top1 + " pts\n" +
                "2. " + top2 + " pts\n" +
                "3. " + top3 + " pts";
        }
    }

    // --- COMPATIBILIDAD ---
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenu(GameObject activeMenu, GameObject desactiveMenu)
    {
        if (activeMenu != null) activeMenu.SetActive(true);
        if (desactiveMenu != null) desactiveMenu.SetActive(false);
    }
}