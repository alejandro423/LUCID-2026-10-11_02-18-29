
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Colores")]
    [SerializeField] private Color colorInactivo = Color.white;
    [SerializeField] private Color colorActivado = Color.cyan;

    [Header("Apariencia")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool activado;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ActualizarColor();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (activado || !otro.CompareTag("Jugador"))
            return;

        if (CheckpointManager.Instancia == null)
        {
            Debug.LogError("No existe CheckpointManager en la escena.");
            return;
        }

        activado = true;

        CheckpointManager.Instancia.ActivarCheckpoint(
            transform.position
        );

        ActualizarColor();
    }

    private void ActualizarColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = activado
                ? colorActivado
                : colorInactivo;
        }
    }
}
