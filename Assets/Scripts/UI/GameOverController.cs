using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Text textoNivel;
    [SerializeField] private Button botonReintentar;
    [SerializeField] private Button botonSeleccion;
    [SerializeField] private Button botonMenu;

    private string nivelGuardado;

    private void Start()
    {
        Time.timeScale = 1f;

        nivelGuardado = PlayerPrefs.GetString("ultimoNivel", "SeleccionNivel");
        if (string.IsNullOrEmpty(nivelGuardado) || nivelGuardado == "GameOver")
            nivelGuardado = "SeleccionNivel";

        if (textoNivel != null)
            textoNivel.text = NombreNivel(nivelGuardado);

        if (botonReintentar != null)
            botonReintentar.onClick.AddListener(Reintentar);

        if (botonSeleccion != null)
            botonSeleccion.onClick.AddListener(IrASeleccion);

        if (botonMenu != null)
            botonMenu.onClick.AddListener(IrAlMenu);
    }

    public void Reintentar()
    {
        SceneManager.LoadScene(nivelGuardado);
    }

    public void IrASeleccion()
    {
        SceneManager.LoadScene("SeleccionNivel");
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private string NombreNivel(string escena)
    {
        switch (escena)
        {
            case "SampleScene": return "NIVEL 1";
            case "JSAB": return "NIVEL 2";
            case "Nivel 3": return "NIVEL 3";
            case "SeleccionNivel": return "SELECCIÓN DE NIVEL";
            default: return escena.ToUpperInvariant();
        }
    }
}
