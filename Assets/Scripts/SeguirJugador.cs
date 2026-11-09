
using UnityEngine;

public class SeguirJugador : MonoBehaviour
{
    [Header("REFERENCIA DEL JUGADOR")]
    [SerializeField] private Transform jugador;

    [Header("EJES DE SEGUIMIENTO")]
    [SerializeField] private bool seguirX = true;
    [SerializeField] private bool seguirY = false;

    [Header("DESPLAZAMIENTO")]
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("MOVIMIENTO")]
    [SerializeField] private bool seguimientoSuave = false;
    [SerializeField] private float velocidadSeguimiento = 5f;

    private void Start()
    {
        // Buscar automáticamente al jugador si no se asignó.
        if (jugador == null)
        {
            GameObject objetoJugador =
                GameObject.FindGameObjectWithTag("Player");

            if (objetoJugador != null)
                jugador = objetoJugador.transform;
        }
    }

    private void LateUpdate()
    {
        if (jugador == null)
            return;

        Vector3 posicionObjetivo = transform.position;

        // Seguimiento horizontal.
        if (seguirX)
            posicionObjetivo.x = jugador.position.x + offset.x;

        // Seguimiento vertical.
        if (seguirY)
            posicionObjetivo.y = jugador.position.y + offset.y;

        // Seguimiento directo o suave.
        if (seguimientoSuave)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                posicionObjetivo,
                velocidadSeguimiento * Time.deltaTime
            );
        }
        else
        {
            transform.position = posicionObjetivo;
        }
    }
}
