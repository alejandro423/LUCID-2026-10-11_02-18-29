using UnityEngine;

public class DerivaNeblina : MonoBehaviour
{
    [SerializeField] private float amplitudHorizontal = 0.55f;
    [SerializeField] private float amplitudVertical = 0.06f;
    [SerializeField] private float velocidad = 0.16f;
    [SerializeField] private float desfase;

    private Vector3 posicionBase;

    private void Awake()
    {
        posicionBase = transform.position;
        if (desfase == 0f)
            desfase = Random.Range(0f, 10f);
    }

    private void Update()
    {
        float t = Time.time * velocidad + desfase;
        transform.position = posicionBase + new Vector3(
            Mathf.Sin(t) * amplitudHorizontal,
            Mathf.Sin(t * 1.7f) * amplitudVertical,
            0f
        );
    }
}
