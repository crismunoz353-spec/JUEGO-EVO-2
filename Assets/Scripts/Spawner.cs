using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    public static Spawner instancia;

    public enum Estado { Jugando, GameOver, Victoria }
    public Estado estado = Estado.Jugando;

    public GameObject enemigoPrefab;

    [Header("Victoria y reinicio")]
    public int enemigosParaGanar = 35;
    public float segundosParaReiniciar = 3f;

    [Header("Oleadas")]
    public int enemigosPorOleada = 3;
    public float tiempoEntreOleadas = 2f;

    [Header("Distancias")]
    [Tooltip("Que tan lejos del borde de la pantalla aparecen")]
    public float margenFueraDePantalla = 1.5f;
    [Tooltip("Distancia minima entre el enemigo y el jugador al aparecer")]
    public float distanciaMinimaAlJugador = 8f;

    float temporizador;
    float tiempoFin;
    int eliminados;
    GUIStyle estiloContador, estiloTitulo, estiloSub;

    void Awake()
    {
        instancia = this;
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Si el juego termino, espera unos segundos y reinicia solo
        if (estado != Estado.Jugando)
        {
            tiempoFin += Time.unscaledDeltaTime;
            if (tiempoFin >= segundosParaReiniciar)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }

        temporizador += Time.deltaTime;
        if (temporizador >= tiempoEntreOleadas)
        {
            temporizador = 0f;
            for (int i = 0; i < enemigosPorOleada; i++)
                CrearEnemigo();
        }
    }

    // Lo llama el enemigo cuando muere
    public void SumarEliminado()
    {
        if (estado != Estado.Jugando) return;

        eliminados++;

        if (eliminados >= enemigosParaGanar)
        {
            estado = Estado.Victoria;
            Time.timeScale = 0f; // congela el juego
        }
    }

    // Lo llama el jugador cuando lo tocan
    public void GameOver()
    {
        if (estado != Estado.Jugando) return;

        estado = Estado.GameOver;
        Time.timeScale = 0f; // congela el juego
    }

    // Dibuja contador y pantallas por codigo, sin Canvas
    void OnGUI()
    {
        if (estiloContador == null)
        {
            estiloContador = new GUIStyle(GUI.skin.label);
            estiloContador.fontSize = 32;
            estiloContador.fontStyle = FontStyle.Bold;
            estiloContador.alignment = TextAnchor.UpperCenter;
            estiloContador.normal.textColor = Color.white;

            estiloTitulo = new GUIStyle(estiloContador);
            estiloTitulo.fontSize = 80;
            estiloTitulo.alignment = TextAnchor.MiddleCenter;

            estiloSub = new GUIStyle(estiloContador);
            estiloSub.fontSize = 30;
            estiloSub.fontStyle = FontStyle.Normal;
            estiloSub.alignment = TextAnchor.MiddleCenter;
        }

        GUI.Label(new Rect(0, 10, Screen.width, 50),
            "Enemigos eliminados: " + eliminados + " / " + enemigosParaGanar, estiloContador);

        if (estado == Estado.Jugando) return;

        // Fondo oscuro semitransparente
        GUI.color = new Color(0f, 0f, 0f, 0.7f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        bool gano = estado == Estado.Victoria;
        estiloTitulo.normal.textColor = gano ? Color.green : Color.red;

        float y = Screen.height * 0.35f;
        GUI.Label(new Rect(0, y, Screen.width, 100), gano ? "¡VICTORIA!" : "GAME OVER", estiloTitulo);

        int restante = Mathf.CeilToInt(segundosParaReiniciar - tiempoFin);
        GUI.Label(new Rect(0, y + 110, Screen.width, 50), "Reiniciando en " + restante + "...", estiloSub);
    }

    void CrearEnemigo()
    {
        Vector3 centro = Jugador.instancia.transform.position;

        float mitadAlto = Camera.main.orthographicSize + margenFueraDePantalla;
        float mitadAncho = Camera.main.orthographicSize * Camera.main.aspect + margenFueraDePantalla;

        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.up;

        float escala = Mathf.Min(mitadAncho / Mathf.Abs(dir.x), mitadAlto / Mathf.Abs(dir.y));
        float distancia = Mathf.Max(escala, distanciaMinimaAlJugador);

        Vector2 posicion = (Vector2)centro + dir * distancia;
        Instantiate(enemigoPrefab, posicion, Quaternion.identity);
    }
}