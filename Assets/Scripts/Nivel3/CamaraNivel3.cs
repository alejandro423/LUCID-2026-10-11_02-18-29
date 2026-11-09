using UnityEngine;

public class CamaraNivel3 : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform objetivo;

    [Header("Seguimiento")]
    [SerializeField] private Vector2 zonaMuerta = new Vector2(1.6f, 0.9f);
    [SerializeField] private float adelantoMirada = 1.3f;
    [SerializeField] private float suavizado = 0.18f;

    [Header("Limites del nivel")]
    [SerializeField] private Vector2 limitesMin = new Vector2(-12f, -4.5f);
    [SerializeField] private Vector2 limitesMax = new Vector2(94f, 8.5f);

    private Camera camara;
    private Rigidbody2D rbObjetivo;
    private Vector3 velocidadSuavizado;
    private Vector2 foco;
    private float direccionMirada = 1f;

    private void Awake()
    {
        camara = GetComponent<Camera>();
        if (objetivo != null)
        {
            rbObjetivo = objetivo.GetComponent<Rigidbody2D>();
            foco = objetivo.position;
            transform.position = new Vector3(foco.x, foco.y, transform.position.z);
        }
    }

    private void LateUpdate()
    {
        if (objetivo == null)
            return;

        Vector2 posicionObjetivo = objetivo.position;

        Vector2 desfase = posicionObjetivo - foco;
        if (Mathf.Abs(desfase.x) > zonaMuerta.x)
            foco.x = posicionObjetivo.x - Mathf.Sign(desfase.x) * zonaMuerta.x;
        if (Mathf.Abs(desfase.y) > zonaMuerta.y)
            foco.y = posicionObjetivo.y - Mathf.Sign(desfase.y) * zonaMuerta.y;

        float velocidadHorizontal = rbObjetivo != null ? rbObjetivo.linearVelocity.x : 0f;
        if (Mathf.Abs(velocidadHorizontal) > 0.1f)
            direccionMirada = Mathf.Sign(velocidadHorizontal);

        float intensidadAdelanto = Mathf.Clamp01(Mathf.Abs(velocidadHorizontal) / 5f);
        Vector2 destino = foco + Vector2.right * (direccionMirada * adelantoMirada * intensidadAdelanto);

        float mitadAlto = camara.orthographicSize;
        float mitadAncho = mitadAlto * camara.aspect;

        float minX = limitesMin.x + mitadAncho;
        float maxX = limitesMax.x - mitadAncho;
        float minY = limitesMin.y + mitadAlto;
        float maxY = limitesMax.y - mitadAlto;

        destino.x = minX <= maxX ? Mathf.Clamp(destino.x, minX, maxX) : (limitesMin.x + limitesMax.x) * 0.5f;
        destino.y = minY <= maxY ? Mathf.Clamp(destino.y, minY, maxY) : (limitesMin.y + limitesMax.y) * 0.5f;

        Vector3 posicionDeseada = new Vector3(destino.x, destino.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidadSuavizado, suavizado);
    }

    public void ReiniciarEn(Vector3 posicion)
    {
        foco = posicion;
        velocidadSuavizado = Vector3.zero;
        transform.position = new Vector3(posicion.x, posicion.y, transform.position.z);
    }
}
