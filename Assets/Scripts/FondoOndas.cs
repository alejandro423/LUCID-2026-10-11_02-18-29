
using UnityEngine;

public class FondoOndas : MonoBehaviour
{
    [Header("Configuracion visual")]
    [SerializeField] private int cantidadOndas = 3;
    [SerializeField] private int puntosPorOnda = 100;
    [SerializeField] private float grosor = 0.018f;
    [SerializeField, Range(0f, 1f)]
    private float opacidad = 0.12f;

    [Header("Movimiento")]
    [SerializeField] private float amplitud = 0.22f;
    [SerializeField] private float frecuencia = 1.4f;
    [SerializeField] private float velocidad = 0.35f;

    [Header("Profundidad")]
    [SerializeField] private float distanciaCamara = 10f;
    [SerializeField] private string capaOrdenacion = "Default";
    [SerializeField] private int ordenRenderizado = -5;

    private LineRenderer[] lineas;
    private Camera camara;
    private Material materialOndas;

    private void Awake()
    {
        camara = Camera.main;

        if (camara == null)
        {
            Debug.LogError(
                "FondoOndas: no se encontro una camara con tag MainCamera."
            );
            enabled = false;
            return;
        }

        Shader shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            Debug.LogError(
                "FondoOndas: no se encontro el shader Sprites/Default."
            );
            enabled = false;
            return;
        }

        cantidadOndas = Mathf.Max(1, cantidadOndas);
        puntosPorOnda = Mathf.Max(2, puntosPorOnda);

        materialOndas = new Material(shader);
        materialOndas.name = "Material_FondoOndas";

        lineas = new LineRenderer[cantidadOndas];

        for (int i = 0; i < cantidadOndas; i++)
        {
            GameObject objeto = new GameObject("Onda_" + i);
            objeto.transform.SetParent(transform, false);

            LineRenderer linea =
                objeto.AddComponent<LineRenderer>();

            linea.useWorldSpace = true;
            linea.positionCount = puntosPorOnda;
            linea.startWidth = grosor;
            linea.endWidth = grosor;
            linea.numCapVertices = 4;
            linea.numCornerVertices = 4;
            linea.textureMode = LineTextureMode.Stretch;
            linea.alignment = LineAlignment.View;
            linea.material = materialOndas;

            linea.sortingLayerName = capaOrdenacion;
            linea.sortingOrder = ordenRenderizado;

            linea.startColor = new Color(1f, 1f, 1f, opacidad);
            linea.endColor = new Color(1f, 1f, 1f, opacidad);

            lineas[i] = linea;
        }
    }

    private void Update()
    {
        if (camara == null || lineas == null)
            return;

        float alto = camara.orthographicSize;
        float ancho = alto * camara.aspect;
        float tiempo = Time.time * velocidad;

        Vector3 centro = camara.transform.position
            + camara.transform.forward * distanciaCamara;

        for (int i = 0; i < lineas.Length; i++)
        {
            float fase = i * 2.1f;

            float altura = Mathf.Lerp(
                -alto * 0.35f,
                alto * 0.35f,
                (i + 1f) / (lineas.Length + 1f)
            );

            for (int p = 0; p < puntosPorOnda; p++)
            {
                float t = p / (float)(puntosPorOnda - 1);

                float x = Mathf.Lerp(-ancho, ancho, t);

                float y = altura
                    + Mathf.Sin(
                        x * frecuencia + tiempo + fase
                    ) * amplitud
                    + Mathf.Sin(
                        x * frecuencia * 0.55f
                        - tiempo * 0.7f + fase
                    ) * amplitud * 0.4f;

                Vector3 punto = centro
                    + camara.transform.right * x
                    + camara.transform.up * y;

                lineas[i].SetPosition(p, punto);
            }
        }
    }

    private void OnDestroy()
    {
        if (materialOndas != null)
        {
            if (Application.isPlaying)
                Destroy(materialOndas);
            else
                DestroyImmediate(materialOndas);
        }
    }
}
