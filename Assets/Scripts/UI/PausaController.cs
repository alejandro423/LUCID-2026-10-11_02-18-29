using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausaController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button botonReanudar;
    [SerializeField] private Button botonReiniciar;
    [SerializeField] private Button botonSeleccion;
    [SerializeField] private Button botonMenu;

    private bool pausado;
    private BeatMusic beatMusic;

    private void Start()
    {
        beatMusic = Object.FindFirstObjectByType<BeatMusic>();

        if (panel != null)
            panel.SetActive(false);

        if (botonReanudar != null)
            botonReanudar.onClick.AddListener(Reanudar);
        if (botonReiniciar != null)
            botonReiniciar.onClick.AddListener(ReiniciarNivel);
        if (botonSeleccion != null)
            botonSeleccion.onClick.AddListener(IrASeleccion);
        if (botonMenu != null)
            botonMenu.onClick.AddListener(IrAlMenu);

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.escapeKey.wasPressedThisFrame)
            AlternarPausa();
    }

    private void AlternarPausa()
    {
        if (pausado)
            Reanudar();
        else
            Pausar();
    }

    public void Pausar()
    {
        pausado = true;

        if (beatMusic != null)
            beatMusic.Pausar();

        if (panel != null)
            panel.SetActive(true);

        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Reanudar()
    {
        pausado = false;

        if (beatMusic != null)
            beatMusic.Reanudar();

        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrASeleccion()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("SeleccionNivel");
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("Menu");
    }

    private void OnDestroy()
    {
        if (pausado)
        {
            if (beatMusic != null)
                beatMusic.Reanudar();

            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}
