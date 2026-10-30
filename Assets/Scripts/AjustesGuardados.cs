using UnityEngine;

/// <summary>
/// Persistencia y aplicación de los ajustes del juego: volumen general,
/// pantalla completa y resolución. Se guardan con PlayerPrefs y se aplican
/// al arrancar (Menú) y en la pantalla de Ajustes.
/// </summary>
public static class AjustesGuardados
{
    private const string ClaveVolumen = "ajustes.volumen";
    private const string ClavePantallaCompleta = "ajustes.pantallaCompleta";
    private const string ClaveResolucion = "ajustes.resolucion";

    /// <summary>Volumen general guardado (0..1). Por defecto 1.</summary>
    public static float Volumen
    {
        get { return PlayerPrefs.GetFloat(ClaveVolumen, 1f); }
    }

    /// <summary>Si el usuario ya eligió pantalla completa alguna vez.</summary>
    public static bool TienePantallaCompleta
    {
        get { return PlayerPrefs.HasKey(ClavePantallaCompleta); }
    }

    /// <summary>Estado guardado de pantalla completa; si nunca se eligió, el estado actual.</summary>
    public static bool PantallaCompleta
    {
        get
        {
            return TienePantallaCompleta
                ? PlayerPrefs.GetInt(ClavePantallaCompleta, 1) == 1
                : Screen.fullScreen;
        }
    }

    public static void GuardarVolumen(float valor)
    {
        PlayerPrefs.SetFloat(ClaveVolumen, Mathf.Clamp01(valor));
        PlayerPrefs.Save();
    }

    public static void GuardarPantallaCompleta(bool activa)
    {
        PlayerPrefs.SetInt(ClavePantallaCompleta, activa ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void GuardarResolucion(int ancho, int alto)
    {
        PlayerPrefs.SetString(ClaveResolucion, ancho + "x" + alto);
        PlayerPrefs.Save();
    }

    /// <summary>Lee la resolución guardada en formato "1280x720".</summary>
    public static bool LeerResolucion(out int ancho, out int alto)
    {
        ancho = 0;
        alto = 0;

        string valor = PlayerPrefs.GetString(ClaveResolucion, string.Empty);
        if (string.IsNullOrEmpty(valor))
            return false;

        string[] partes = valor.Split('x');
        if (partes.Length != 2)
            return false;

        return int.TryParse(partes[0], out ancho)
            && int.TryParse(partes[1], out alto)
            && ancho > 0
            && alto > 0;
    }

    /// <summary>Aplica los ajustes guardados (se llama al cargar el Menú).</summary>
    public static void Aplicar()
    {
        AudioListener.volume = Volumen;

        if (TienePantallaCompleta)
            Screen.fullScreen = PantallaCompleta;

        int ancho;
        int alto;
        if (LeerResolucion(out ancho, out alto))
            Screen.SetResolution(ancho, alto, Screen.fullScreen);
    }
}
