using UnityEngine;

public class HazardFactory : MonoBehaviour
{
    public Sprite squareSprite;
    public Sprite triangleSprite;
    public Sprite bulletSprite;
    public Sprite beamSprite;
    public Sprite ringSprite;
    public Material spriteMaterial;
    public Transform container;
    public PlayerController player;

    Camera cam;

    public float HalfWidth { get; private set; }
    public float HalfHeight { get; private set; }

    void Awake()
    {
        cam = Camera.main;
        HalfHeight = cam.orthographicSize;
        HalfWidth = HalfHeight * cam.aspect;
    }

    public Vector2 RandomEdgePoint(float margin)
    {
        int e = Random.Range(0, 4);
        float w = HalfWidth + margin;
        float h = HalfHeight + margin;
        switch (e)
        {
            case 0: return new Vector2(Random.Range(-w, w), h);
            case 1: return new Vector2(Random.Range(-w, w), -h);
            case 2: return new Vector2(-w, Random.Range(-h, h));
            default: return new Vector2(w, Random.Range(-h, h));
        }
    }

    public Bullet SpawnBullet(Vector2 position, Vector2 direction, float speed)
    {
        var go = new GameObject("Bullet");
        go.transform.SetParent(container, false);
        go.transform.position = position;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = spriteMaterial;
        sr.sprite = bulletSprite;
        sr.sortingOrder = 6;
        var col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.18f;
        var bullet = go.AddComponent<Bullet>();
        bullet.velocity = direction.normalized * speed;
        bullet.lifeTime = 8f;
        return bullet;
    }

    public Bouncer SpawnBouncer(Vector2 position, Vector2 velocity, float size, float lifeSeconds)
    {
        var go = new GameObject("Bouncer");
        go.transform.SetParent(container, false);
        go.transform.position = position;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = spriteMaterial;
        sr.sprite = squareSprite;
        sr.sortingOrder = 6;
        Vector2 boundsSize = sr.sprite.bounds.size;
        float scale = size / boundsSize.x;
        go.transform.localScale = Vector3.one * scale;
        var box = go.AddComponent<BoxCollider2D>();
        box.size = boundsSize;
        var bouncer = go.AddComponent<Bouncer>();
        bouncer.velocity = velocity;
        bouncer.boundX = Mathf.Max(1f, HalfWidth - size * 0.5f - 0.1f);
        bouncer.boundY = Mathf.Max(1f, HalfHeight - size * 0.5f - 0.1f);
        bouncer.spinDegreesPerSecond = Random.Range(90f, 220f) * (Random.value < 0.5f ? -1f : 1f);
        bouncer.lifeTime = lifeSeconds;
        return bouncer;
    }

    public Beam SpawnBeam(Vector2 center, float length, float thickness, bool vertical, float telegraphSeconds, float solidSeconds)
    {
        var go = new GameObject("Beam");
        go.transform.SetParent(container, false);
        go.transform.position = center;
        if (vertical)
            go.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = spriteMaterial;
        sr.sprite = beamSprite;
        sr.sortingOrder = 5;
        Vector2 boundsSize = sr.sprite.bounds.size;
        go.transform.localScale = new Vector3(length / boundsSize.x, thickness / boundsSize.y, 1f);
        var box = go.AddComponent<BoxCollider2D>();
        box.size = boundsSize;
        var beam = go.AddComponent<Beam>();
        beam.telegraphSeconds = telegraphSeconds;
        beam.solidSeconds = solidSeconds;
        beam.lifeTime = telegraphSeconds + solidSeconds + 0.25f;
        return beam;
    }

    public RingWave SpawnRing(Vector2 center, float rotationZ, float telegraphSeconds, float expandSpeed)
    {
        var go = new GameObject("Ring");
        go.transform.SetParent(container, false);
        go.transform.position = center;
        go.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = spriteMaterial;
        sr.sprite = ringSprite;
        sr.sortingOrder = 5;
        var ring = go.AddComponent<RingWave>();
        ring.player = player;
        ring.telegraphSeconds = telegraphSeconds;
        ring.expandSpeed = expandSpeed;
        return ring;
    }

    public void SpawnTriangleTelegraph(Vector2 center, float duration)
    {
        var go = new GameObject("Telegraph");
        go.transform.SetParent(container, false);
        go.transform.position = center;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = spriteMaterial;
        sr.sprite = triangleSprite;
        sr.sortingOrder = 5;
        sr.color = new Color(1f, 1f, 1f, 0.55f);
        var hazard = go.AddComponent<Hazard>();
        hazard.spinDegreesPerSecond = 220f;
        hazard.lifeTime = duration;
        hazard.fadeSeconds = 0.12f;
    }
}
