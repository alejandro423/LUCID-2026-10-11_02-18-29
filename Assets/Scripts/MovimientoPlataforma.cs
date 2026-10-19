
using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    [Header("Recorrido")]
    [SerializeField] private bool usarCoordenadas = true;
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private Vector2 coordenadaA = new Vector2(0f, 0f);
    [SerializeField] private Vector2 coordenadaB = new Vector2(5f, 0f);

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;

    private Vector2 destinoA;
    private Vector2 destinoB;
    private Vector2 destinoActual;

    private Rigidbody2D rbJugador;
    private Transform jugadorSobrePlataforma;

    private Rigidbody2D rbPlataforma;
    private Vector2 posicionAnterior;

    private void Awake()
    {
        rbPlataforma = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (usarCoordenadas)
        {
            destinoA = coordenadaA;
            destinoB = coordenadaB;
        }
        else
        {
            if (puntoA == null || puntoB == null)
            {
                Debug.LogError(
                    "Asigna Punto A y Punto B o activa Usar Coordenadas.",
                    this
                );
                enabled = false;
                return;
            }

            destinoA = puntoA.position;
            destinoB = puntoB.position;
        }

        posicionAnterior = transform.position;
        destinoActual = destinoB;
    }

    private void FixedUpdate()
    {
        Vector2 posicionActual = rbPlataforma != null
            ? rbPlataforma.position
            : (Vector2)transform.position;

        Vector2 nuevaPosicion = Vector2.MoveTowards(
            posicionActual,
            destinoActual,
            velocidad * Time.fixedDeltaTime
        );

        if (rbPlataforma != null)
        {
            rbPlataforma.MovePosition(nuevaPosicion);
        }
        else
        {
            transform.position = new Vector3(
                nuevaPosicion.x,
                nuevaPosicion.y,
                transform.position.z
            );
        }

        if (Vector2.Distance(nuevaPosicion, destinoActual) < 0.01f)
        {
            destinoActual = destinoActual == destinoA
                ? destinoB
                : destinoA;
        }

        Vector2 desplazamiento = nuevaPosicion - posicionActual;

        // Transportar al jugador sin cambiar su jerarquia.
        if (jugadorSobrePlataforma != null && rbJugador != null)
        {
            rbJugador.position += desplazamiento;
        }

        posicionAnterior = nuevaPosicion;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Jugador"))
            return;

        foreach (ContactPoint2D punto in collision.contacts)
        {
            // La normal apunta hacia arriba en la superficie
            // de la plataforma cuando el jugador esta encima.
            if (punto.normal.y < -0.5f)
            {
                jugadorSobrePlataforma = collision.transform;
                rbJugador = collision.rigidbody;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (jugadorSobrePlataforma == collision.transform)
        {
            jugadorSobrePlataforma = null;
            rbJugador = null;
        }
    }
}
