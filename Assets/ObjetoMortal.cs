
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

        SistemaVidas sistema = SistemaVidas.Instancia;
        bool gameOver = false;

        if (sistema != null)
        {
            gameOver = sistema.PerderVida();
        }
        else
        {
            Debug.LogError("No existe SistemaVidas en la escena.");
        }

        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (efectoMuerte != null)
        {
            Instantiate(
                efectoMuerte,
                jugador.transform.position,
                Quaternion.identity
            );
        }

        Vector3 escalaInicial = jugador.transform.localScale;

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

        yield return new WaitForSeconds(tiempoReaparicion);

        // Si se acabaron las vidas, ir a GameOver.
        if (gameOver)
        {
            SceneManager.LoadScene("GameOver");
            yield break;
        }

        // Obtener el ultimo checkpoint o el punto inicial.
        Vector3 posicionReaparicion;

        if (CheckpointManager.Instancia != null)
        {
            posicionReaparicion =
                CheckpointManager.Instancia.ObtenerPosicionReaparicion();
        }
        else
        {
            GameObject puntoInicio = GameObject.Find("PuntoInicio");

            if (puntoInicio == null)
            {
                Debug.LogError("No se encontro PuntoInicio.");
                yield break;
            }

            posicionReaparicion = puntoInicio.transform.position;
        }

        // Reaparecer al jugador.
        jugador.transform.position = posicionReaparicion;
        jugador.transform.localScale = escalaInicial;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = true;
        }

        if (sistema != null)
        {
            sistema.FinalizarMuerte();
        }

        muriendo = false;
    }
}
