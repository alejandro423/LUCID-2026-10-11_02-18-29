using System.Collections;
using UnityEngine;

public class Beam : Hazard
{
    public float telegraphSeconds = 0.5f;
    public float solidSeconds = 0.5f;

    BoxCollider2D box;

    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        if (box != null)
            box.enabled = false;
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0.28f;
            spriteRenderer.color = c;
        }
        spinDegreesPerSecond = 0f;
        StartCoroutine(Solidify());
    }

    IEnumerator Solidify()
    {
        yield return new WaitForSeconds(telegraphSeconds);
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }
    }

    void Update()
    {
        Tick();
        if (box != null && spriteRenderer != null)
            box.enabled = spriteRenderer.color.a > 0.9f;
    }
}
