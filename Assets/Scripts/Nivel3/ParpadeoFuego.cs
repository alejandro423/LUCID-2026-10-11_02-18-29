using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParpadeoFuego : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light2D luz;

    [Header("Parpadeo visual")]
    [SerializeField] private float amplitudEscala = 0.08f;
    [SerializeField] private float velocidad = 7f;

    [Header("Luz")]
    [SerializeField] private float intensidadBase = 1.1f;
    [SerializeField] private float amplitudLuz = 0.3f;

    private Vector3 escalaBase;
    private float semilla;

    private void Awake()
    {
        escalaBase = transform.localScale;
        semilla = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float t = Time.time * velocidad + semilla;
        float ruido = Mathf.PerlinNoise(t, semilla * 0.37f) * 2f - 1f;

        transform.localScale = escalaBase * (1f + ruido * amplitudEscala);

        if (luz != null)
            luz.intensity = intensidadBase + ruido * amplitudLuz;
    }
}
