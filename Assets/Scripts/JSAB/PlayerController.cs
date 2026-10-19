using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 8.5f;
    public int maxHp = 3;
    public float invulnerableSeconds = 1.2f;
    public Sprite[] animFrames;
    public float animFps = 10f;
    public float idlePulseAmount = 0.03f;
    public float idlePulseSpeed = 2f;

    public int Hp { get; private set; }
    public bool Dead { get; private set; }
    public float HitRadius { get; private set; }
    public event Action OnDead;
    public event Action OnHit;

    SpriteRenderer spriteRenderer;
    Camera cam;
    float invuln;
    float animTimer;
    int animIndex;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        HitRadius = GetComponent<CircleCollider2D>().radius;
        Hp = maxHp;
    }

    void Update()
    {
        if (Dead)
            return;

        Vector2 input = ReadMovement();
        if (input.sqrMagnitude > 1f)
            input.Normalize();
        transform.position += (Vector3)(input * speed * Time.deltaTime);

        if (input.x < -0.01f)
            spriteRenderer.flipX = true;
        else if (input.x > 0.01f)
            spriteRenderer.flipX = false;

        if (animFrames != null && animFrames.Length > 0)
        {
            animTimer += Time.deltaTime * (input.sqrMagnitude > 0.01f ? animFps : animFps * 0.4f);
            int next = Mathf.FloorToInt(animTimer) % animFrames.Length;
            if (next != animIndex && animFrames[next] != null)
            {
                animIndex = next;
                spriteRenderer.sprite = animFrames[animIndex];
            }
        }

        float pulse = 1f + idlePulseAmount * (0.5f + 0.5f * Mathf.Sin(Time.time * idlePulseSpeed));
        transform.localScale = new Vector3(pulse, pulse, 1f);

        float halfH = cam.orthographicSize - 0.6f;
        float halfW = halfH * cam.aspect;
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -halfW, halfW);
        p.y = Mathf.Clamp(p.y, -halfH, halfH);
        transform.position = p;

        if (invuln > 0f)
        {
            invuln -= Time.deltaTime;
            spriteRenderer.enabled = Mathf.Repeat(Time.time * 12f, 1f) < 0.5f;
            if (invuln <= 0f)
                spriteRenderer.enabled = true;
        }
    }

    Vector2 ReadMovement()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        float x = 0f;
        float y = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            y += 1f;

        return new Vector2(x, y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Hazard>(out _))
            TakeDamage();
    }

    public void TakeDamage()
    {
        if (invuln > 0f || Dead)
            return;
        Hp--;
        invuln = invulnerableSeconds;
        OnHit?.Invoke();
        if (Hp <= 0)
            Die();
    }

    void Die()
    {
        Dead = true;
        spriteRenderer.enabled = false;
        OnDead?.Invoke();
    }

    public void ResetPlayer()
    {
        Hp = maxHp;
        Dead = false;
        invuln = 2f;
        transform.position = new Vector3(0f, -2f, 0f);
        spriteRenderer.enabled = true;
    }
}
