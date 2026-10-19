using UnityEngine;

public class Bullet : Hazard
{
    public Vector2 velocity;

    void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
        Tick();
        if (Mathf.Abs(transform.position.x) > 30f || Mathf.Abs(transform.position.y) > 22f)
            Destroy(gameObject);
    }
}
