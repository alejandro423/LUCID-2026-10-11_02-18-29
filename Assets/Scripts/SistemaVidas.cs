
using UnityEngine;

public class SistemaVidas : MonoBehaviour
{
    public static SistemaVidas Instancia { get; private set; }

    [Header("Configuracion de vidas")]
    [SerializeField] private int vidasMaximas = 3;

    private int vidasActuales;
    private bool procesandoMuerte;

    public int VidasActuales => vidasActuales;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        vidasMaximas = Mathf.Max(1, vidasMaximas);
        vidasActuales = vidasMaximas;
    }

    public bool PerderVida()
    {
        if (procesandoMuerte)
            return false;

        procesandoMuerte = true;
        vidasActuales = Mathf.Max(0, vidasActuales - 1);

        Debug.Log("Vidas restantes: " + vidasActuales);

        // Devuelve true si se acabaron las vidas.
        return vidasActuales <= 0;
    }

    public void FinalizarMuerte()
    {
        procesandoMuerte = false;
    }
}
