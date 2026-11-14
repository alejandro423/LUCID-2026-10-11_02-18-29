
using UnityEngine;

public class ObjetoDesvanecible : MonoBehaviour
{
    public enum EstadoJugador
    {
        EnMovimiento,
        Quieto
    }

    [Header("REFERENCIA DEL JUGADOR")]
    [SerializeField] private Rigidbody2D jugador;
    [SerializeField] private float umbralMovimiento = 0.1f;

    [Header("COMPORTAMIENTO")]
    [SerializeField] private EstadoJugador aparecerCuando =
        EstadoJugador.EnMovimiento;

    [SerializeField] private bool permitirDesaparecerConJugadorEncima = true;

    [Tooltip("Si está activado, el jugador podrá permanecer sobre el objeto mientras desaparece.")]
    [SerializeField] private bool mantenerHitboxDuranteDesaparicion = true;

    [Header("VELOCIDAD DE TRANSICION")]
    [Tooltip("Unidades de opacidad por segundo")]
    [SerializeField] private float velocidadAparicion = 2f;

    [Tooltip("Unidades de opacidad por segundo")]
    [SerializeField] private float velocidadDesaparicion = 2f;

    [Header("OPACIDAD")]
    [SerializeField] private AnimationCurve curvaOpacidad =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.7f, 0.15f),
            new Keyframe(1f, 1f)
        );

    [Header("COMPONENTES")]
    [SerializeField] private SpriteRenderer[] renderers;
    [SerializeField] private Collider2D[] hitboxes;

    [Header("ESTADO INICIAL")]
    [SerializeField] private bool iniciarVisible = true;

    private Color[] coloresOriginales;
    private Collider2D colliderJugador;

    private float opacidadActual;
    private float opacidadInicial;
    private float opacidadObjetivo;

    private float progreso;
    private float duracion;

    private bool objetivoVisible;
    private bool transicionando;

    private void Awake()
    {
        // Buscar jugador automáticamente.
        if (jugador == null)
        {
            GameObject objetoJugador =
                GameObject.FindGameObjectWithTag("Jugador");

            if (objetoJugador != null)
            {
                jugador = objetoJugador.GetComponent<Rigidbody2D>();
            }
        }

        // Obtener collider del jugador.
        if (jugador != null)
        {
            colliderJugador = jugador.GetComponent<Collider2D>();

            if (colliderJugador == null)
            {
                colliderJugador =
                    jugador.GetComponentInChildren<Collider2D>();
            }
        }

        // Buscar SpriteRenderer.
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<SpriteRenderer>(true);
        }

        // Buscar hitboxes.
        if (hitboxes == null || hitboxes.Length == 0)
        {
            hitboxes = GetComponentsInChildren<Collider2D>(true);
        }

        // Guardar colores originales.
        coloresOriginales = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                coloresOriginales[i] = renderers[i].color;
            }
        }
    }

    private void Start()
    {
        objetivoVisible = iniciarVisible;
        opacidadActual = iniciarVisible ? 1f : 0f;

        AplicarOpacidad();
        ActualizarHitbox();
    }

    private void Update()
    {
        bool jugadorMoviendose = EstaEnMovimiento();

        // Determinar cuándo debe aparecer el objeto.
        bool debeEstarVisible =
            aparecerCuando == EstadoJugador.EnMovimiento
            ? jugadorMoviendose
            : !jugadorMoviendose;

        // Comprobar si necesita cambiar de estado.
        if (debeEstarVisible != objetivoVisible)
        {
            // Comprobar si puede desaparecer con el jugador encima.
            if (!debeEstarVisible &&
                !permitirDesaparecerConJugadorEncima &&
                EstaTocandoObjeto())
            {
                if (transicionando)
                {
                    IniciarTransicion(true);
                }

                return;
            }

            IniciarTransicion(debeEstarVisible);
        }

        // Actualizar transición visual.
        if (transicionando)
        {
            ActualizarTransicion();
        }
    }

    private bool EstaEnMovimiento()
    {
        if (jugador == null)
            return false;

        Vector2 velocidad = jugador.linearVelocity;

        // Movimiento horizontal.
        bool movimientoHorizontal =
            Mathf.Abs(velocidad.x) > umbralMovimiento;

        // Movimiento vertical ascendente.
        bool movimientoAscendente =
            velocidad.y > umbralMovimiento;

        return movimientoHorizontal || movimientoAscendente;
    }

    private bool EstaTocandoObjeto()
    {
        if (colliderJugador == null)
            return false;

        foreach (Collider2D hitbox in hitboxes)
        {
            if (hitbox == null || !hitbox.enabled)
                continue;

            if (colliderJugador.IsTouching(hitbox))
                return true;
        }

        return false;
    }

    private void IniciarTransicion(bool aparecer)
    {
        objetivoVisible = aparecer;

        opacidadInicial = opacidadActual;
        opacidadObjetivo = aparecer ? 1f : 0f;

        progreso = 0f;

        float velocidad = aparecer
            ? velocidadAparicion
            : velocidadDesaparicion;

        float diferencia = Mathf.Abs(
            opacidadObjetivo - opacidadInicial
        );

        if (velocidad <= 0f || diferencia <= 0.001f)
        {
            opacidadActual = opacidadObjetivo;
            transicionando = false;

            AplicarOpacidad();
            ActualizarHitbox();

            return;
        }

        duracion = diferencia / velocidad;

        transicionando = true;

        if (aparecer)
        {
            // Al aparecer, el hitbox permanece desactivado
            // hasta que el objeto sea completamente visible.
            DesactivarHitbox();
        }
        else if (!mantenerHitboxDuranteDesaparicion)
        {
            // Comportamiento opcional: desactivar hitbox
            // desde el comienzo de la desaparición.
            DesactivarHitbox();
        }
    }

    private void ActualizarTransicion()
    {
        progreso += Time.deltaTime / duracion;
        progreso = Mathf.Clamp01(progreso);

        float valorCurva = curvaOpacidad.Evaluate(progreso);

        opacidadActual = Mathf.Lerp(
            opacidadInicial,
            opacidadObjetivo,
            valorCurva
        );

        AplicarOpacidad();

        if (progreso >= 1f)
        {
            opacidadActual = opacidadObjetivo;
            transicionando = false;

            AplicarOpacidad();

            // El hitbox se desactiva al finalizar
            // la desaparición, no al comenzarla.
            ActualizarHitbox();
        }
    }

    private void AplicarOpacidad()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
                continue;

            Color color = coloresOriginales[i];
            color.a *= opacidadActual;

            renderers[i].color = color;
        }
    }

    private void DesactivarHitbox()
    {
        foreach (Collider2D hitbox in hitboxes)
        {
            if (hitbox != null)
                hitbox.enabled = false;
        }
    }

    private void ActualizarHitbox()
    {
        bool habilitar =
            objetivoVisible &&
            !transicionando &&
            opacidadActual >= 0.999f;

        foreach (Collider2D hitbox in hitboxes)
        {
            if (hitbox != null)
                hitbox.enabled = habilitar;
        }
    }
}
