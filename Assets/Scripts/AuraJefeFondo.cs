
using UnityEngine;

public class AuraJefeFondo : MonoBehaviour
{
    [Header("APARIENCIA")]
    [SerializeField] private Color colorAura =
        new Color(0.65f, 0.85f, 1f, 1f);

    [Range(0f, 3f)]
    [SerializeField] private float intensidad = 1.5f;

    [SerializeField] private float tamanoAura = 8f;

    [Header("PULSACION")]
    [SerializeField] private float velocidadPulso = 1f;

    [SerializeField] private float variacionTamano = 0.15f;

    [SerializeField] private float variacionIntensidad = 0.2f;

    [Header("CAPAS")]
    [SerializeField] private int cantidadCapas = 6;

    [SerializeField] private float separacionCapas = 0.18f;

    [Header("ORDEN DE RENDERIZADO")]
    [SerializeField] private string sortingLayer = "Fondo";

    [SerializeField] private int sortingOrder = -20;

    [SerializeField] private int incrementoOrden = 1;

    private SpriteRenderer[] capas;
    private float[] opacidades;

    private void Start()
    {
        CrearAura();
    }

    private void CrearAura()
    {
        capas = new SpriteRenderer[cantidadCapas];
        opacidades = new float[cantidadCapas];

        for (int i = 0; i < cantidadCapas; i++)
        {
            GameObject capa = new GameObject("CapaAura_" + i);

            capa.transform.SetParent(transform, false);

            float escala = 1f + i * separacionCapas;

            capa.transform.localScale =
                Vector3.one * escala;

            SpriteRenderer sr =
                capa.AddComponent<SpriteRenderer>();

            sr.sprite = CrearSpriteCircular();

            // Cada capa se dibuja detrás de la anterior.
            sr.sortingLayerName = sortingLayer;
            sr.sortingOrder = sortingOrder - (i * incrementoOrden);

            capas[i] = sr;

            // Las capas exteriores son más transparentes.
            opacidades[i] =
                (1f - (float)i / cantidadCapas) * 0.16f;
        }
    }

    private Sprite CrearSpriteCircular()
    {
        Texture2D textura = new Texture2D(128, 128);

        textura.filterMode = FilterMode.Bilinear;
        textura.wrapMode = TextureWrapMode.Clamp;

        Vector2 centro = new Vector2(63.5f, 63.5f);

        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                float distancia =
                    Vector2.Distance(new Vector2(x, y), centro) / 64f;

                float alpha =
                    Mathf.Pow(Mathf.Clamp01(1f - distancia), 3f);

                textura.SetPixel(
                    x, y,
                    new Color(1f, 1f, 1f, alpha)
                );
            }
        }

        textura.Apply();

        return Sprite.Create(
            textura,
            new Rect(0, 0, 128, 128),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }

    private void Update()
    {
        if (capas == null)
            return;

        float pulso = Mathf.Sin(Time.time * velocidadPulso);

        float escalaActual =
            tamanoAura * (1f + pulso * variacionTamano);

        float intensidadActual =
            intensidad * (1f + pulso * variacionIntensidad);

        for (int i = 0; i < capas.Length; i++)
        {
            if (capas[i] == null)
                continue;

            float escala = 1f + i * separacionCapas;

            capas[i].transform.localScale =
                Vector3.one * escala * escalaActual;

            Color color = colorAura;

            color.a = Mathf.Clamp01(
                opacidades[i] * intensidadActual
            );

            capas[i].color = color;
        }
    }
}
