using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarNivel : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private string nombreEscena = "nivel 2";

    private bool cambiando = false;

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (cambiando || !otro.CompareTag("Jugador"))
            return;

        cambiando = true;

        SceneManager.LoadScene(nombreEscena);
    }
}
