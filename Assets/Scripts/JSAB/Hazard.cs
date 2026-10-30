using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float spinDegreesPerSecond = 120f;
    public float lifeTime = 6f;
    public float fadeSeconds = 0.25f;

    protected float age;
    protected SpriteRenderer spriteRenderer;

    bool fading;
    float baseAlpha;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Tick()
    {
        age += Time.deltaTime;
        transform.Rotate(0f, 0f, spinDegreesPerSecond * Time.deltaTime);
        float remaining = lifeTime - age;
        if (remaining < fadeSeconds && spriteRenderer != null)
        {
            if (!fading)
            {
                fading = true;
                baseAlpha = spriteRenderer.color.a;
            }
            Color c = spriteRenderer.color;
            c.a = baseAlpha * Mathf.Clamp01(remaining / fadeSeconds);
            spriteRenderer.color = c;
        }
        if (age >= lifeTime)
            Destroy(gameObject);
    }
}
