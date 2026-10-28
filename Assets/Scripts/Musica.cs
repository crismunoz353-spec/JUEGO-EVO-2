using UnityEngine;

public class Musica : MonoBehaviour
{
    static Musica instancia;

    [Header("Sonidos (arrastra aqui los audios)")]
    public AudioClip musicaFondo;
    public AudioClip sonidoGameOver;
    public AudioClip sonidoVictoria;

    [Header("Volumen")]
    [Range(0f, 1f)] public float volumenMusica = 0.4f;
    [Range(0f, 1f)] public float volumenEfectos = 0.8f;

    AudioSource fuenteFondo;
    AudioSource fuenteFinal;
    Spawner.Estado estadoAnterior;

    void Start()
    {
        // Si por error hay dos scripts Musica en la escena, deja solo uno
        if (instancia != null && instancia != this)
        {
            Destroy(this);
            return;
        }
        instancia = this;

        // Detiene cualquier otro AudioSource de la escena que suene por su cuenta
        // (por ejemplo, audios que se arrastraron directo a la Hierarchy)
        foreach (AudioSource otro in FindObjectsOfType<AudioSource>())
        {
            otro.Stop();
            otro.playOnAwake = false;
        }

        // Una fuente para la musica de fondo y otra para game over / victoria
        fuenteFondo = CrearFuente(true, volumenMusica);
        fuenteFinal = CrearFuente(false, volumenEfectos);

        estadoAnterior = Spawner.instancia.estado;

        if (musicaFondo != null)
        {
            fuenteFondo.clip = musicaFondo;
            fuenteFondo.Play();
        }
    }

    void Update()
    {
        if (fuenteFondo == null) return;

        Spawner.Estado actual = Spawner.instancia.estado;
        if (actual == estadoAnterior) return;

        estadoAnterior = actual;

        // Siempre se apaga primero la musica de fondo, y despues suena solo UN audio final
        if (actual == Spawner.Estado.GameOver)
            ReproducirFinal(sonidoGameOver);
        else if (actual == Spawner.Estado.Victoria)
            ReproducirFinal(sonidoVictoria);
    }

    void ReproducirFinal(AudioClip clip)
    {
        fuenteFondo.Stop();
        fuenteFinal.Stop();

        if (clip == null) return;

        fuenteFinal.clip = clip;
        fuenteFinal.Play();
    }

    AudioSource CrearFuente(bool repetir, float volumen)
    {
        AudioSource fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.loop = repetir;
        fuente.volume = volumen;
        return fuente;
    }
}