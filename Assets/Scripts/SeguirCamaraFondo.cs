
using UnityEngine;

public class SeguirCamaraFondo : MonoBehaviour
{
    [Header("REFERENCIA")]
    [SerializeField] private Transform camara;

    [Header("POSICION EN PANTALLA")]
    [SerializeField] private Vector2 desplazamiento =
        new Vector2(0f, 0f);

    [Header("EFECTO DE PROFUNDIDAD")]
    [Range(0f, 1f)]
    [SerializeField] private float seguimiento = 1f;

    private Vector3 posicionInicial;

    private void Start()
    {
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;

        if (camara != null)
        {
            posicionInicial = transform.position;
        }
    }

    private void LateUpdate()
    {
        if (camara == null)
            return;

        Vector3 objetivo = new Vector3(
            camara.position.x + desplazamiento.x,
            camara.position.y + desplazamiento.y,
            posicionInicial.z
        );

        Vector3 posicionActual = transform.position;

        transform.position = new Vector3(
            Mathf.Lerp(
                posicionInicial.x,
                objetivo.x,
                seguimiento
            ),
            Mathf.Lerp(
                posicionInicial.y,
                objetivo.y,
                seguimiento
            ),
            posicionInicial.z
        );
    }
}
