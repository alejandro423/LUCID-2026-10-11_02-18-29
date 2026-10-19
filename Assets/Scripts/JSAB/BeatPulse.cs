using UnityEngine;

public class BeatPulse : MonoBehaviour
{
    public BeatMusic beatMusic;
    public Color baseColor = new Color(0.075f, 0.075f, 0.157f, 1f);
    public Color flashColor = new Color(0.11f, 0.11f, 0.22f, 1f);

    SpriteRenderer spriteRenderer;
    float pulse;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (beatMusic != null)
            beatMusic.OnBeat += Beat;
    }

    void OnDisable()
    {
        if (beatMusic != null)
            beatMusic.OnBeat -= Beat;
    }

    void Beat(int beat)
    {
        pulse = 1f;
    }

    void Update()
    {
        pulse = Mathf.Max(0f, pulse - Time.deltaTime * 2.5f);
        if (spriteRenderer != null)
            spriteRenderer.color = Color.Lerp(baseColor, flashColor, pulse);
    }
}
