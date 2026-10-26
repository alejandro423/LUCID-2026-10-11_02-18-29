
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlSeleccionNivel : MonoBehaviour
{

    public void VolverAlMenu()
    {
        AudioManager.Clic();
        SceneManager.LoadScene("Menu");
    }

    public void AbrirNivel1()
    {
        AudioManager.Clic();
        SceneManager.LoadScene("SampleScene");
    }

    public void AbrirNivel2()
    {
        AudioManager.Clic();
        SceneManager.LoadScene("JSAB");
    }
}
