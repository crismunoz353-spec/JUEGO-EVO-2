using UnityEngine;

public class Jugador : MonoBehaviour
{
    public static Jugador instancia;

    public GameObject balaPrefab;
    public float radio = 0.4f;

    void Awake()
    {
        instancia = this;
    }

    void Update()
    {
        // Si el juego termino (game over o victoria), no se puede apuntar ni disparar
        if (Spawner.instancia.estado != Spawner.Estado.Jugando) return;

        // Apuntar hacia el mouse
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direccion = mouse - transform.position;
        transform.up = direccion.normalized;

        // Disparar con click
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(balaPrefab, transform.position + transform.up * 0.6f, transform.rotation);
        }
    }

    // Lo llama el enemigo cuando toca la nave
    public void Morir()
    {
        Spawner.instancia.GameOver();
    }
}