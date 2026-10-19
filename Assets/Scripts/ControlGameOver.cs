
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlGameOver : MonoBehaviour
{
    public void Reintentar()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Salir()
    {
        SceneManager.LoadScene("Menu");
    }
}
