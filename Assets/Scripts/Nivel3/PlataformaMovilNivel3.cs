using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlataformaMovilNivel3 : MonoBehaviour
{
    [Header("Recorrido")]
    [SerializeField] private Vector2 puntoA;
    [SerializeField] private Vector2 puntoB;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 1.9f;
    [SerializeField] private float pausaEnExtremos = 0.5f;

    private Rigidbody2D cuerpo;
    private Vector2 destino;
    private float espera;
    private Rigidbody2D jugador;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        cuerpo.bodyType = RigidbodyType2D.Kinematic;
        cuerpo.interpolation = RigidbodyInterpolation2D.Interpolate;
        cuerpo.useFullKinematicContacts = true;
    }

    private void Start()
    {
        transform.position = puntoA;
        cuerpo.position = puntoA;
        destino = puntoB;
    }

    private void FixedUpdate()
    {
        if (espera > 0f)
        {
            espera -= Time.fixedDeltaTime;
            return;
        }

        Vector2 actual = cuerpo.position;
        Vector2 nueva = Vector2.MoveTowards(actual, destino, velocidad * Time.fixedDeltaTime);
        cuerpo.MovePosition(nueva);

        if (jugador != null && jugador.simulated)
            jugador.position += nueva - actual;

        if (Vector2.Distance(nueva, destino) < 0.01f)
        {
            espera = pausaEnExtremos;
            destino = Vector2.Distance(destino, puntoA) < 0.01f ? puntoB : puntoA;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
            jugador = collision.rigidbody;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
            jugador = null;
    }
}
