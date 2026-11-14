using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class CamaraResponsiva2D : MonoBehaviour
{
    [SerializeField] private float aspectoReferencia = 16f / 9f;

    private Camera camara;
    private int anchoActual;
    private int altoActual;

    private void Awake()
    {
        camara = GetComponent<Camera>();
        AjustarEncuadre();
    }

    private void LateUpdate()
    {
        if (Screen.width != anchoActual || Screen.height != altoActual)
            AjustarEncuadre();
    }

    private void AjustarEncuadre()
    {
        anchoActual = Screen.width;
        altoActual = Screen.height;

        if (anchoActual <= 0 || altoActual <= 0)
            return;

        float aspectoActual = (float)anchoActual / altoActual;
        float relacion = aspectoActual / aspectoReferencia;

        if (relacion < 1f)
        {
            camara.rect = new Rect(0f, (1f - relacion) * 0.5f, 1f, relacion);
        }
        else
        {
            float ancho = 1f / relacion;
            camara.rect = new Rect((1f - ancho) * 0.5f, 0f, ancho, 1f);
        }
    }
}
