using System.Collections;
using UnityEngine;

public class HUDPlataformas : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private HUDController hud;
    [SerializeField] private UnityEngine.UI.Text titulo;
    [SerializeField] private UnityEngine.UI.Text pista;
    [SerializeField] private UnityEngine.UI.Image marcaCheckpoint;
    [SerializeField] private Transform jugador;
    [SerializeField] private Transform inicio;
    [SerializeField] private Transform fin;

    [Header("Contenido")]
    [SerializeField] private string nombreNivel = "NIVEL 1";
    [SerializeField] private string textoPista = "\u2190 \u2192  MOVER     S  SALTAR     A  DASH";
    [SerializeField] private float duracionTitulo = 4f;
    [SerializeField] private float duracionPista = 8f;

    private int vidasAnterior = -1;
    private int vidasMaxAnterior = -1;
    private Vector3 ultimoCheckpoint = new Vector3(float.NaN, 0f, 0f);

    private void Start()
    {
        if (titulo != null)
        {
            titulo.text = nombreNivel;
            StartCoroutine(FundirTexto(titulo, duracionTitulo));
        }

        if (pista != null)
        {
            pista.text = textoPista;
            StartCoroutine(FundirTexto(pista, duracionPista));
        }

        if (marcaCheckpoint != null)
            marcaCheckpoint.enabled = false;

        ActualizarVidas(true);
        ActualizarProgreso();
        ActualizarCheckpoint();
    }

    private void Update()
    {
        ActualizarVidas(false);
        ActualizarProgreso();
        ActualizarCheckpoint();
    }

    private void ActualizarVidas(bool forzar)
    {
        var sistema = SistemaVidas.Instancia;
        if (sistema == null || hud == null)
            return;

        if (!forzar && sistema.VidasActuales == vidasAnterior && sistema.VidasMaximas == vidasMaxAnterior)
            return;

        vidasAnterior = sistema.VidasActuales;
        vidasMaxAnterior = sistema.VidasMaximas;
        hud.SetHp(vidasAnterior, vidasMaxAnterior);
    }

    private void ActualizarProgreso()
    {
        if (hud == null || jugador == null || inicio == null || fin == null)
            return;

        float largo = fin.position.x - inicio.position.x;
        if (Mathf.Abs(largo) < 0.01f)
            return;

        hud.SetProgress(Mathf.Clamp01((jugador.position.x - inicio.position.x) / largo));
    }

    private void ActualizarCheckpoint()
    {
        var manager = CheckpointManager.Instancia;
        if (manager == null || hud == null || !manager.CheckpointActivado)
            return;

        Vector3 posicion = manager.ObtenerPosicionReaparicion();
        if (!float.IsNaN(ultimoCheckpoint.x) && Mathf.Abs(posicion.x - ultimoCheckpoint.x) < 0.01f)
            return;

        ultimoCheckpoint = posicion;

        if (inicio != null && fin != null)
        {
            float largo = fin.position.x - inicio.position.x;
            if (Mathf.Abs(largo) > 0.01f)
                hud.SetCheckpointMark(Mathf.Clamp01((posicion.x - inicio.position.x) / largo));
        }

        if (marcaCheckpoint != null)
            marcaCheckpoint.enabled = true;

        hud.FlashMessage("CHECKPOINT");
    }

    private IEnumerator FundirTexto(UnityEngine.UI.Text texto, float espera)
    {
        if (texto == null)
            yield break;

        yield return new WaitForSecondsRealtime(espera);
        texto.CrossFadeAlpha(0f, 0.8f, true);
    }
}
