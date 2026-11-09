using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CheckpointNivel3 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform llama;
    [SerializeField] private Light2D luz;

    [Header("Apariencia")]
    [SerializeField] private Color colorActivo = new Color(0.35f, 0.85f, 1f, 1f);

    private bool activado;
    private Vector3 escalaBase;
    private float intensidadBase;

    private void Awake()
    {
        if (llama == null)
            llama = transform.Find("Flame");

        if (llama != null)
            escalaBase = llama.localScale;

        if (luz != null)
            intensidadBase = luz.intensity;
    }

    private void Update()
    {
        if (activado || llama == null)
            return;

        float pulso = 1f + 0.04f * Mathf.Sin(Time.time * 4f);
        llama.localScale = escalaBase * pulso;

        if (luz != null)
            luz.intensity = intensidadBase * (1f + 0.08f * Mathf.Sin(Time.time * 5f));
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (activado || !otro.CompareTag("Jugador"))
            return;

        if (CheckpointManager.Instancia == null)
        {
            Debug.LogError("No existe CheckpointManager en la escena.");
            return;
        }

        activado = true;
        CheckpointManager.Instancia.ActivarCheckpoint(transform.position);

        if (luz != null)
            luz.color = colorActivo;

        StartCoroutine(PulsoActivacion());
    }

    private IEnumerator PulsoActivacion()
    {
        float duracion = 0.6f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracion);
            float curva = Mathf.Sin(progreso * Mathf.PI);

            if (llama != null)
            {
                llama.localScale = escalaBase * (1f + 0.6f * curva);
                var sr = llama.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = Color.Lerp(colorActivo, Color.white, progreso * 0.3f);
            }

            if (luz != null)
                luz.intensity = intensidadBase + 1.8f * curva;

            yield return null;
        }

        if (llama != null)
            llama.localScale = escalaBase * 1.15f;

        if (luz != null)
            luz.intensity = intensidadBase * 1.25f;
    }
}
