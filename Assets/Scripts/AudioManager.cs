using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la música de cada escena y los sonidos del juego.
/// Se crea solo al darle Play (no hay que ponerlo en ninguna escena)
/// y sobrevive entre escenas.
/// Los efectos de sonido se generan por código, no necesitan archivos.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia { get; private set; }

    private const float VolumenMusica = 0.6f;
    private const float VolumenEfectos = 0.8f;
    private const float VelocidadFade = 1.5f;
    private const int FrecuenciaMuestreo = 44100;

    private enum Onda { Seno, Cuadrada, Triangulo, Ruido }

    private AudioSource fuenteMusica;
    private AudioSource fuenteEfectos;
    private AudioClip musicaObjetivo;
    private readonly Dictionary<string, AudioClip> sonidos = new Dictionary<string, AudioClip>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Crear()
    {
        if (Instancia != null)
            return;

        GameObject objeto = new GameObject("AudioManager");
        DontDestroyOnLoad(objeto);
        objeto.AddComponent<AudioManager>();
    }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteMusica.loop = true;
        fuenteMusica.playOnAwake = false;
        fuenteMusica.spatialBlend = 0f;
        fuenteMusica.volume = 0f;

        fuenteEfectos = gameObject.AddComponent<AudioSource>();
        fuenteEfectos.playOnAwake = false;
        fuenteEfectos.spatialBlend = 0f;

        GenerarSonidos();

        SceneManager.sceneLoaded += AlCargarEscena;
        AplicarEscena(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instancia == this)
            SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        AplicarEscena(escena.name);
    }

    // ------------------------------------------------------------
    //  MUSICA POR ESCENA
    //  Los clips se cargan desde Assets/Resources/Musica/
    // ------------------------------------------------------------
    private void AplicarEscena(string nombreEscena)
    {
        switch (nombreEscena)
        {
            case "Menu":
            case "SeleccionNivel":
            case "Ajustes":
                musicaObjetivo = CargarMusica("Musica/nivel 0");
                break;

            case "GameOver":
                musicaObjetivo = null;
                Reproducir("gameover");
                break;

            // SampleScene (nivel 1) ya tiene su objeto MusicaFondo.
            // JSAB (nivel 2) usa su propio BeatMusic sincronizado.
            default:
                musicaObjetivo = null;
                break;
        }
    }

    private AudioClip CargarMusica(string ruta)
    {
        AudioClip clip = Resources.Load<AudioClip>(ruta);

        if (clip == null)
        {
            Debug.LogWarning(
                "AudioManager: no se encontro Assets/Resources/" + ruta + ".mp3"
            );
        }

        return clip;
    }

    private void Update()
    {
        float paso = VelocidadFade * Time.unscaledDeltaTime;

        if (fuenteMusica.clip != musicaObjetivo)
        {
            // Bajar el volumen y despues cambiar de pista.
            fuenteMusica.volume = Mathf.MoveTowards(fuenteMusica.volume, 0f, paso);

            if (fuenteMusica.volume <= 0.001f)
            {
                fuenteMusica.Stop();
                fuenteMusica.clip = musicaObjetivo;

                if (musicaObjetivo != null)
                    fuenteMusica.Play();
            }
        }
        else if (musicaObjetivo != null)
        {
            fuenteMusica.volume = Mathf.MoveTowards(
                fuenteMusica.volume, VolumenMusica, paso
            );
        }
    }

    // ------------------------------------------------------------
    //  SONIDOS (llamar desde cualquier script)
    // ------------------------------------------------------------
    public static void Salto()       { Reproducir("salto"); }
    public static void DobleSalto()  { Reproducir("dobleSalto"); }
    public static void Dash()        { Reproducir("dash"); }
    public static void Aterrizar()   { Reproducir("aterrizar"); }
    public static void Muerte()      { Reproducir("muerte"); }
    public static void Golpe()       { Reproducir("golpe"); }
    public static void Clic()        { Reproducir("clic"); }
    public static void Checkpoint()  { Reproducir("checkpoint"); }
    public static void Victoria()    { Reproducir("victoria"); }

    private static void Reproducir(string nombre)
    {
        if (Instancia == null)
            return;

        AudioClip clip;

        if (Instancia.sonidos.TryGetValue(nombre, out clip))
            Instancia.fuenteEfectos.PlayOneShot(clip, VolumenEfectos);
    }

    // ------------------------------------------------------------
    //  GENERACION DE EFECTOS
    // ------------------------------------------------------------
    private void GenerarSonidos()
    {
        Registrar("salto",
            Tono(0.16f, 320f, 720f, Onda.Cuadrada, 9f, 0.30f));

        Registrar("dobleSalto",
            Tono(0.18f, 520f, 1100f, Onda.Triangulo, 8f, 0.50f));

        Registrar("dash", Mezclar(
            Tono(0.18f, 0f, 0f, Onda.Ruido, 14f, 0.30f),
            Tono(0.18f, 300f, 90f, Onda.Seno, 10f, 0.40f)));

        Registrar("aterrizar",
            Tono(0.07f, 150f, 70f, Onda.Seno, 25f, 0.50f));

        Registrar("muerte", Mezclar(
            Tono(0.40f, 420f, 50f, Onda.Cuadrada, 6f, 0.30f),
            Tono(0.40f, 0f, 0f, Onda.Ruido, 8f, 0.25f)));

        Registrar("golpe", Mezclar(
            Tono(0.20f, 250f, 80f, Onda.Cuadrada, 12f, 0.35f),
            Tono(0.20f, 0f, 0f, Onda.Ruido, 14f, 0.25f)));

        Registrar("clic",
            Tono(0.05f, 900f, 650f, Onda.Seno, 40f, 0.40f));

        Registrar("checkpoint", Unir(
            Tono(0.10f, 660f, 660f, Onda.Triangulo, 6f, 0.45f),
            Tono(0.18f, 880f, 880f, Onda.Triangulo, 6f, 0.45f)));

        Registrar("victoria", Unir(
            Tono(0.12f, 523f, 523f, Onda.Triangulo, 5f, 0.45f),
            Tono(0.12f, 659f, 659f, Onda.Triangulo, 5f, 0.45f),
            Tono(0.12f, 784f, 784f, Onda.Triangulo, 5f, 0.45f),
            Tono(0.35f, 1047f, 1047f, Onda.Triangulo, 4f, 0.45f)));

        Registrar("gameover", Unir(
            Tono(0.25f, 392f, 392f, Onda.Triangulo, 4f, 0.45f),
            Tono(0.25f, 330f, 330f, Onda.Triangulo, 4f, 0.45f),
            Tono(0.25f, 262f, 262f, Onda.Triangulo, 4f, 0.45f),
            Tono(0.50f, 196f, 160f, Onda.Triangulo, 3f, 0.45f)));
    }

    private void Registrar(string nombre, float[] datos)
    {
        AudioClip clip = AudioClip.Create(
            nombre, datos.Length, 1, FrecuenciaMuestreo, false
        );
        clip.SetData(datos, 0);
        sonidos[nombre] = clip;
    }

    private static float[] Tono(
        float duracion, float frecuenciaInicial, float frecuenciaFinal,
        Onda onda, float caida, float volumen)
    {
        int total = Mathf.RoundToInt(duracion * FrecuenciaMuestreo);
        float[] datos = new float[total];
        float fase = 0f;
        System.Random azar = new System.Random(7);
        int muestrasFinal = Mathf.Max(1, Mathf.RoundToInt(0.01f * FrecuenciaMuestreo));

        for (int i = 0; i < total; i++)
        {
            float t = i / (float)FrecuenciaMuestreo;
            float avance = i / (float)total;
            float frecuencia = Mathf.Lerp(frecuenciaInicial, frecuenciaFinal, avance);

            fase += frecuencia / FrecuenciaMuestreo;
            float p = fase - Mathf.Floor(fase);
            float muestra;

            switch (onda)
            {
                case Onda.Cuadrada:
                    muestra = p < 0.5f ? 0.6f : -0.6f;
                    break;
                case Onda.Triangulo:
                    muestra = 4f * Mathf.Abs(p - 0.5f) - 1f;
                    break;
                case Onda.Ruido:
                    muestra = (float)(azar.NextDouble() * 2.0 - 1.0);
                    break;
                default:
                    muestra = Mathf.Sin(2f * Mathf.PI * p);
                    break;
            }

            float ataque = Mathf.Clamp01(t / 0.004f);
            float cierre = Mathf.Clamp01((total - i) / (float)muestrasFinal);
            float envolvente = ataque * cierre * Mathf.Exp(-t * caida);

            datos[i] = muestra * envolvente * volumen;
        }

        return datos;
    }

    private static float[] Mezclar(float[] a, float[] b)
    {
        float[] resultado = new float[Mathf.Max(a.Length, b.Length)];

        for (int i = 0; i < a.Length; i++)
            resultado[i] += a[i];

        for (int i = 0; i < b.Length; i++)
            resultado[i] += b[i];

        return resultado;
    }

    private static float[] Unir(params float[][] partes)
    {
        int total = 0;

        foreach (float[] parte in partes)
            total += parte.Length;

        float[] resultado = new float[total];
        int posicion = 0;

        foreach (float[] parte in partes)
        {
            parte.CopyTo(resultado, posicion);
            posicion += parte.Length;
        }

        return resultado;
    }
}
