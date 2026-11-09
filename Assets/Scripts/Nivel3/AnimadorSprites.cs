using UnityEngine;

public class AnimadorSprites : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SpriteRenderer destino;
    [SerializeField] private Sprite[] fotogramas;

    [Header("Reproduccion")]
    [SerializeField] private float fps = 10f;
    [SerializeField] private bool faseAleatoria = true;
    [SerializeField] private float desfase;

    private float tiempo;
    private int indice = -1;

    private void Awake()
    {
        if (destino == null)
            destino = GetComponent<SpriteRenderer>();

        if (fotogramas == null || fotogramas.Length == 0 || destino == null)
        {
            enabled = false;
            return;
        }

        if (faseAleatoria)
            desfase = Random.value / Mathf.Max(fps, 0.01f) * fotogramas.Length;

        tiempo = desfase;
        indice = Mathf.FloorToInt(tiempo * fps) % fotogramas.Length;
        destino.sprite = fotogramas[indice];
    }

    private void Update()
    {
        if (destino == null || fotogramas.Length < 2)
            return;

        tiempo += Time.deltaTime;
        int nuevo = Mathf.FloorToInt(tiempo * fps) % fotogramas.Length;
        if (nuevo == indice)
            return;

        indice = nuevo;
        destino.sprite = fotogramas[indice];
    }
}
