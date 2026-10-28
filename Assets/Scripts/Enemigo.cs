using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    // Lista de todos los enemigos vivos (la usa la bala para detectarlos)
    public static List<Enemigo> todos = new List<Enemigo>();

    public float velocidad = 2f;
    public int vidas = 2;
    public float radio = 0.45f;

    bool muerto;
    SpriteRenderer sprite;

    void OnEnable()
    {
        todos.Add(this);
    }

    void OnDisable()
    {
        todos.Remove(this);
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Avanza hacia el centro (donde esta la nave)
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, velocidad * Time.deltaTime);

        // Si toca a la nave, el jugador muere
        float distancia = Vector2.Distance(transform.position, Jugador.instancia.transform.position);
        if (distancia < radio + Jugador.instancia.radio)
        {
            Jugador.instancia.Morir();
        }
    }

    // Lo llama la bala cuando lo golpea
    public void RecibirDano()
    {
        if (muerto) return;

        vidas--;

        if (vidas <= 0)
        {
            muerto = true;
            Spawner.instancia.SumarEliminado();
            Destroy(gameObject);
        }
        else if (sprite != null)
        {
            sprite.color = Color.red; // herido
        }
    }
}