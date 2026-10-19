using UnityEngine;

public class RingWave : MonoBehaviour
{
    public PlayerController player;
    public float telegraphSeconds = 0.5f;
    public float expandSpeed = 5.5f;
    public float startRadius = 1.2f;
    public float maxRadius = 9.5f;
    public float damageThickness = 0.55f;
    public float gapHalfAngle = 30f;

    SpriteRenderer spriteRenderer;
    float baseRadius;
    float radius;
    float age;
    bool solid;
    float gapCenterDeg;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseRadius = spriteRenderer.sprite.bounds.extents.x * 0.96f;
        gapCenterDeg = transform.eulerAngles.z;
        radius = startRadius;
        transform.localScale = Vector3.one * (radius / baseRadius);
        Color c = spriteRenderer.color;
        c.a = 0.3f;
        spriteRenderer.color = c;
    }

    void Update()
    {
        age += Time.deltaTime;
        if (!solid)
        {
            if (age < telegraphSeconds)
                return;
            solid = true;
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        radius += expandSpeed * Time.deltaTime;
        transform.localScale = Vector3.one * (radius / baseRadius);

        if (player != null && !player.Dead)
        {
            Vector2 d = player.transform.position - transform.position;
            float dist = d.magnitude;
            if (Mathf.Abs(dist - radius) < damageThickness + player.HitRadius)
            {
                float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
                if (Mathf.Abs(Mathf.DeltaAngle(angle, gapCenterDeg)) > gapHalfAngle)
                    player.TakeDamage();
            }
        }

        if (radius >= maxRadius)
            Destroy(gameObject);
    }
}
