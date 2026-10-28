using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidad = 12f;
    public float tiempoDeVida = 3f;
    public float radio = 0.15f;

    [Header("Sonido")]
    public AudioClip sonidoDisparo;
    [Range(0f, 1f)] public float volumen = 0.6f;

    void Start()
    {
        // La bala nace justo al disparar, asi que aqui suena el disparo
        if (sonidoDisparo != null)
            AudioSource.PlayClipAtPoint(sonidoDisparo, Camera.main.transform.position, volumen);

        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        // Avanza recto hacia donde apunta la bala
        transform.position += transform.up * velocidad * Time.deltaTime;

        // Revisa por distancia si toca a algun enemigo (no usa colliders)
        foreach (Enemigo e in Enemigo.todos)
        {
            if (e == null) continue;

            float distancia = Vector2.Distance(transform.position, e.transform.position);
            if (distancia < radio + e.radio)
            {
                e.RecibirDano();
                Destroy(gameObject);
                return;
            }
        }
    }
}