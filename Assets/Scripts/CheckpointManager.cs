
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instancia { get; private set; }

    private Vector3 posicionInicial;
    private Vector3 posicionCheckpoint;
    private bool checkpointActivado;

    public bool CheckpointActivado => checkpointActivado;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        GameObject puntoInicio = GameObject.Find("PuntoInicio");

        if (puntoInicio != null)
        {
            posicionInicial = puntoInicio.transform.position;
        }
        else
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Jugador");

            if (jugador != null)
                posicionInicial = jugador.transform.position;
            else
                Debug.LogError("No se encontro el jugador ni PuntoInicio.");
        }

        posicionCheckpoint = posicionInicial;
    }

    public void ActivarCheckpoint(Vector3 posicion)
    {
        posicionCheckpoint = posicion;
        checkpointActivado = true;

        if (SistemaVidas.Instancia != null)
            SistemaVidas.Instancia.RestaurarVidas();

        Debug.Log("Checkpoint activado. Vidas restauradas.");
    }

    public Vector3 ObtenerPosicionReaparicion()
    {
        return checkpointActivado
            ? posicionCheckpoint
            : posicionInicial;
    }

    private void OnDestroy()
    {
        if (Instancia == this)
            Instancia = null;
    }
}
