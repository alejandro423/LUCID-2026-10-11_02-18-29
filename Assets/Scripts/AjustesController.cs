using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controla la pantalla de Ajustes: volumen general, pantalla completa y
/// resolución. Los cambios se aplican al instante y se guardan con
/// <see cref="AjustesGuardados"/>. La escena se construye desde el Menú.
/// </summary>
public class AjustesController : MonoBehaviour
{
    [Header("Audio")]
    public Slider volumenSlider;
    public TMP_Text volumenPorcentaje;

    [Header("Video")]
    public Toggle pantallaCompletaToggle;
    public TMP_Text resolucionTexto;
    public Button resolucionAnterior;
    public Button resolucionSiguiente;

    [Header("Navegación")]
    public Button volverBoton;

    private readonly List<Vector2Int> resoluciones = new List<Vector2Int>();
    private int resolucionIndice;

    private void Start()
    {
        AjustesGuardados.Aplicar();
        CargarResoluciones();

        ConfigurarVolumen();
        ConfigurarPantallaCompleta();
        ConfigurarResolucion();

        if (volverBoton != null)
            volverBoton.onClick.AddListener(Volver);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Volver();
    }

    private void ConfigurarVolumen()
    {
        float volumen = AjustesGuardados.Volumen;

        volumenSlider.SetValueWithoutNotify(volumen);
        volumenSlider.onValueChanged.AddListener(OnVolumenCambiado);

        ActualizarTextoVolumen(volumen);
    }

    private void OnVolumenCambiado(float valor)
    {
        AudioListener.volume = valor;
        AjustesGuardados.GuardarVolumen(valor);
        ActualizarTextoVolumen(valor);
    }

    private void ActualizarTextoVolumen(float valor)
    {
        if (volumenPorcentaje != null)
            volumenPorcentaje.text = Mathf.RoundToInt(valor * 100f) + "%";
    }

    private void ConfigurarPantallaCompleta()
    {
        pantallaCompletaToggle.SetIsOnWithoutNotify(AjustesGuardados.PantallaCompleta);
        pantallaCompletaToggle.onValueChanged.AddListener(OnPantallaCompletaCambiada);
    }

    private void OnPantallaCompletaCambiada(bool activa)
    {
        Screen.fullScreen = activa;
        AjustesGuardados.GuardarPantallaCompleta(activa);
    }

    private void ConfigurarResolucion()
    {
        resolucionIndice = IndiceInicialResolucion();
        ActualizarTextoResolucion();

        if (resolucionAnterior != null)
            resolucionAnterior.onClick.AddListener(() => CambiarResolucion(-1));

        if (resolucionSiguiente != null)
            resolucionSiguiente.onClick.AddListener(() => CambiarResolucion(1));
    }

    /// <summary>Lista de resoluciones válidas (sin repetir y ordenadas).</summary>
    private void CargarResoluciones()
    {
        resoluciones.Clear();

        HashSet<long> vistas = new HashSet<long>();

        foreach (Resolution resolucion in Screen.resolutions)
        {
            if (resolucion.width < 1024 || resolucion.height < 600)
                continue;

            long clave = ((long)resolucion.width << 32) | (uint)resolucion.height;
            if (!vistas.Add(clave))
                continue;

            resoluciones.Add(new Vector2Int(resolucion.width, resolucion.height));
        }

        resoluciones.Sort((a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

        if (resoluciones.Count == 0)
            resoluciones.Add(new Vector2Int(Screen.width, Screen.height));
    }

    private int IndiceInicialResolucion()
    {
        int ancho;
        int alto;
        if (AjustesGuardados.LeerResolucion(out ancho, out alto))
        {
            int guardado = resoluciones.FindIndex(r => r.x == ancho && r.y == alto);
            if (guardado >= 0)
                return guardado;
        }

        int actual = resoluciones.FindIndex(r => r.x == Screen.width && r.y == Screen.height);
        return actual >= 0 ? actual : resoluciones.Count - 1;
    }

    private void CambiarResolucion(int paso)
    {
        if (resoluciones.Count <= 1)
            return;

        resolucionIndice = (resolucionIndice + paso + resoluciones.Count) % resoluciones.Count;
        ActualizarTextoResolucion();

        Vector2Int resolucion = resoluciones[resolucionIndice];
        Screen.SetResolution(resolucion.x, resolucion.y, Screen.fullScreen);
        AjustesGuardados.GuardarResolucion(resolucion.x, resolucion.y);
    }

    private void ActualizarTextoResolucion()
    {
        if (resolucionTexto == null || resoluciones.Count == 0)
            return;

        Vector2Int resolucion = resoluciones[resolucionIndice];
        resolucionTexto.text = resolucion.x + " x " + resolucion.y;
    }

    public void Volver()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
}
