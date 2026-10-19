using UnityEngine;

public class Bouncer : Hazard
{
    public Vector2 velocity;
    public float boundX = 8.5f;
    public float boundY = 4.5f;

    void Update()
    {
        Vector3 p = transform.position + (Vector3)(velocity * Time.deltaTime);
        if (p.x < -boundX) { p.x = -boundX; velocity.x = Mathf.Abs(velocity.x); }
        else if (p.x > boundX) { p.x = boundX; velocity.x = -Mathf.Abs(velocity.x); }
        if (p.y < -boundY) { p.y = -boundY; velocity.y = Mathf.Abs(velocity.y); }
        else if (p.y > boundY) { p.y = boundY; velocity.y = -Mathf.Abs(velocity.y); }
        transform.position = p;
        Tick();
    }
}
