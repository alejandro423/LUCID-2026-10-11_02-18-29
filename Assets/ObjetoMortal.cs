using UnityEngine;
using System.Collections;

public class ObjetoMortal : MonoBehaviour
{
    [Header("Efecto de muerte")]
    [SerializeField] private GameObject efectoMuerte;

    [Header("Muerte")]
    [SerializeField] private float duracionMuerte = 0.15f;
    [SerializeField] private float tiempoReaparicion = 0.3f;

    private bool muriendo = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Jugador") && !muriendo)
        {
            StartCoroutine(Morir(collision));
        }
    }

    private IEnumerator Morir(Collider2D jugador)
    {
        muriendo = true;

        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();

        // Detener al jugador
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Crear efecto de partículas
        if (efectoMuerte != null)
        {
            Instantiate(
                efectoMuerte,
                jugador.transform.position,
                Quaternion.identity
            );
        }

        // Guardar tamaño original
        Vector3 escalaInicial = jugador.transform.localScale;

        // Hacer desaparecer al jugador
        float tiempo = 0f;

        while (tiempo < duracionMuerte)
        {
            tiempo += Time.deltaTime;

            float progreso = tiempo / duracionMuerte;

            jugador.transform.localScale =
                Vector3.Lerp(
                    escalaInicial,
                    Vector3.zero,
                    progreso
                );

            yield return null;
        }

        // Asegurarnos de que desaparezca completamente
        jugador.transform.localScale = Vector3.zero;

        // Esperar antes de reaparecer
        yield return new WaitForSeconds(tiempoReaparicion);

        // Buscar el punto inicial
        GameObject puntoInicio = GameObject.Find("PuntoInicio");

        if (puntoInicio != null)
        {
            jugador.transform.position = puntoInicio.transform.position;
        }
        else
        {
            Debug.LogError("No se encontró el objeto PuntoInicio.");
        }

        // Restaurar tamaño
        jugador.transform.localScale = escalaInicial;

        // Reactivar física
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        muriendo = false;
    }

private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Jugador") && !muriendo)
    {
        StartCoroutine(Morir(collision.collider));
    }
}

}