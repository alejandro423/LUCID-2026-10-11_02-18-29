
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MovimientoVuelo : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField]
    private float velocidad = 5f;

    [Header("Dash")]
    [SerializeField]
    private float fuerzaDash = 15f;

    [SerializeField]
    private float duracionDash = 0.15f;

    [SerializeField]
    private float delayDash = 0.5f;

    [Header("Efecto visual del Dash")]
    [SerializeField]
    private Transform visualPersonaje;

    [SerializeField]
    private float escalaLargo = 1.5f;

    [SerializeField]
    private float escalaAncho = 0.65f;

    [SerializeField]
    private float velocidadTransformacion = 15f;

    [Header("Rotacion del personaje")]
    [SerializeField]
    private Transform pivoteRotacion;

    [SerializeField]
    private float velocidadRotacion = 12f;

    [SerializeField]
    private float anguloInicial = 90f;

    private Vector3 escalaOriginal;

    private Rigidbody2D rb;

    // Dirección actual de movimiento
    private Vector2 direccionMovimiento;

    // Dirección actual del dash
    private Vector2 direccionDash;

    // Última dirección válida
    private Vector2 ultimaDireccion = Vector2.up;

    // Ángulo objetivo de rotación
    private float anguloObjetivo;

    // Carga del dash
    private int cargaDash = 1;

    private bool haciendoDash = false;
    private bool esperandoDash = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // El jugador vuela sin gravedad
        rb.gravityScale = 0f;

        // Guardar escala original
        if (visualPersonaje != null)
        {
            escalaOriginal = visualPersonaje.localScale;
        }

        // Inicializar rotación del pivote
        if (pivoteRotacion != null)
        {
            anguloObjetivo = 0f;

            pivoteRotacion.localRotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        LeerMovimiento();
        Dash();
        ActualizarEfectoDash();
        ActualizarRotacion();
    }

    private void FixedUpdate()
    {
        Mover();
    }

    private void LeerMovimiento()
    {
        if (Keyboard.current == null)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.leftArrowKey.isPressed)
            horizontal = -1f;

        if (Keyboard.current.rightArrowKey.isPressed)
            horizontal = 1f;

        if (Keyboard.current.upArrowKey.isPressed)
            vertical = 1f;

        if (Keyboard.current.downArrowKey.isPressed)
            vertical = -1f;

        direccionMovimiento = new Vector2(horizontal, vertical);

        // Evitar mayor velocidad en diagonal
        direccionMovimiento = Vector2.ClampMagnitude(
            direccionMovimiento,
            1f
        );

        if (direccionMovimiento != Vector2.zero)
        {
            ultimaDireccion = direccionMovimiento;

            // Durante el dash actualizar su dirección
            if (haciendoDash)
            {
                direccionDash = direccionMovimiento.normalized;
            }
        }
    }

    private void Mover()
    {
        if (haciendoDash)
        {
            // Mantener velocidad del dash en su dirección actual
            rb.linearVelocity = direccionDash * fuerzaDash;
        }
        else
        {
            rb.linearVelocity = direccionMovimiento * velocidad;
        }
    }

    private void Dash()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.aKey.wasPressedThisFrame
            && cargaDash == 1
            && !haciendoDash
            && !esperandoDash)
        {
            cargaDash = 0;

            // Usar dirección actual o última dirección válida
            direccionDash = direccionMovimiento != Vector2.zero
                ? direccionMovimiento.normalized
                : ultimaDireccion.normalized;

            AudioManager.Dash();

            StartCoroutine(EjecutarDash());
        }
    }

    private IEnumerator EjecutarDash()
    {
        haciendoDash = true;

        yield return new WaitForSeconds(duracionDash);

        haciendoDash = false;

        StartCoroutine(RecargarDash());
    }

    private IEnumerator RecargarDash()
    {
        esperandoDash = true;

        yield return new WaitForSeconds(delayDash);

        esperandoDash = false;

        cargaDash = 1;
    }

    private void ActualizarEfectoDash()
    {
        if (visualPersonaje == null)
            return;

        Vector3 escalaObjetivo = escalaOriginal;

        if (haciendoDash)
        {
            // Dirección horizontal
            if (Mathf.Abs(direccionDash.x) >
                Mathf.Abs(direccionDash.y))
            {
                escalaObjetivo.x = escalaOriginal.x * escalaLargo;
                escalaObjetivo.y = escalaOriginal.y * escalaAncho;
            }
            // Dirección vertical
            else
            {
                escalaObjetivo.x = escalaOriginal.x * escalaAncho;
                escalaObjetivo.y = escalaOriginal.y * escalaLargo;
            }
        }

        // Transformación suave
        visualPersonaje.localScale = Vector3.Lerp(
            visualPersonaje.localScale,
            escalaObjetivo,
            velocidadTransformacion * Time.deltaTime
        );
    }

    private void ActualizarRotacion()
    {
        if (pivoteRotacion == null)
            return;

        Vector2 direccionObjetivo;

        if (haciendoDash)
        {
            // Durante el dash mirar hacia su dirección actual
            direccionObjetivo = direccionDash;
        }
        else
        {
            // Fuera del dash mirar hacia el movimiento
            direccionObjetivo = direccionMovimiento;
        }

        // Si no hay movimiento, regresar a la orientación original
        if (direccionObjetivo == Vector2.zero)
        {
            anguloObjetivo = 0f;
        }
        else
        {
            float angulo = Mathf.Atan2(
                direccionObjetivo.y,
                direccionObjetivo.x
            ) * Mathf.Rad2Deg;

            // Compensar orientación original del sprite
            anguloObjetivo = angulo - anguloInicial;
        }

        // Rotación progresiva
        float nuevoAngulo = Mathf.LerpAngle(
            pivoteRotacion.localEulerAngles.z,
            anguloObjetivo,
            velocidadRotacion * Time.deltaTime
        );

        pivoteRotacion.localRotation =
            Quaternion.Euler(0f, 0f, nuevoAngulo);
    }
}
