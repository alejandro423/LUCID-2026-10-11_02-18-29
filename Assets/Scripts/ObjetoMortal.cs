
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador") && !muriendo)
        {
            StartCoroutine(Morir(collision.collider));
        }
    }

    private IEnumerator Morir(Collider2D jugador)
    {
        muriendo = true;

        // Descontar una vida.
        SistemaVidas sistema = SistemaVidas.Instancia;
        bool gameOver = false;

        if (sistema != null)
        {
            gameOver = sistema.PerderVida();
        }
        else
        {
            Debug.LogError(
                "No existe SistemaVidas en la escena."
            );
        }

        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();

        // Detener al jugador.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Crear efecto de particulas.
        if (efectoMuerte != null)
        {
            Instantiate(
                efectoMuerte,
                jugador.transform.position,
                Quaternion.identity
            );
        }

        // Guardar el tamaño original.
        Vector3 escalaInicial = jugador.transform.localScale;

        // Hacer desaparecer al jugador.
        float tiempo = 0f;

        while (tiempo < duracionMuerte)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(
                tiempo / duracionMuerte
            );

            jugador.transform.localScale = Vector3.Lerp(
                escalaInicial,
                Vector3.zero,
                progreso
            );

            yield return null;
        }

        jugador.transform.localScale = Vector3.zero;

        // Esperar antes de reaparecer o ir a Game Over.
        yield return new WaitForSeconds(tiempoReaparicion);

        // Si no quedan vidas, cargar Game Over.
        if (gameOver)
        {
            SceneManager.LoadScene("GameOver");
            yield break;
        }

        // Buscar el punto inicial.
        GameObject puntoInicio = GameObject.Find("PuntoInicio");

        if (puntoInicio != null)
        {
            jugador.transform.position = puntoInicio.transform.position;
        }
        else
        {
            Debug.LogError(
                "No se encontró el objeto PuntoInicio."
            );
        }

        // Restaurar el tamaño.
        jugador.transform.localScale = escalaInicial;

        // Reactivar la fisica.
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        if (sistema != null)
        {
            sistema.FinalizarMuerte();
        }

        muriendo = false;
    }
}
