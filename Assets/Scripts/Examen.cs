using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class BouncingSquare : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 8f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        // Lanzar con una diagonal bien marcada desde el inicio
        LanzarAleatorio();
    }

    void LanzarAleatorio()
    {
        float x = Random.Range(0, 2) == 0 ? 1f : -1f;
        float y = Random.Range(0, 2) == 0 ? 1f : -1f;
        
        Vector2 dir = new Vector2(x, y).normalized;
        rb.linearVelocity = dir * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0];
        Vector2 reflectDir = Vector2.Reflect(rb.linearVelocity.normalized, contact.normal);

        // --- TRUCO ANTI-ATASCO ---
        // Si el rebote hace que se mueva casi plano (horizontal o vertical),
        // le forzamos un pequeño ángulo para que obligatoriamente se mueva en ambas direcciones.
        if (Mathf.Abs(reflectDir.x) < 0.4f)
        {
            reflectDir.x = reflectDir.x > 0 ? 0.6f : -0.6f;
        }
        if (Mathf.Abs(reflectDir.y) < 0.4f)
        {
            reflectDir.y = reflectDir.y > 0 ? 0.6f : -0.6f;
        }
        // -------------------------

        rb.linearVelocity = reflectDir.normalized * speed;
    }

    void FixedUpdate()
    {
        // Asegurar que la velocidad se mantenga constante siempre
        if (rb.linearVelocity.magnitude > 0)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
        else
        {
            LanzarAleatorio();
        }
    }
}