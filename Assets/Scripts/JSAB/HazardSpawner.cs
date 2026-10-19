using System.Collections;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public BeatMusic beatMusic;
    public HazardFactory factory;
    public PlayerController player;
    public int introBeats = 2;

    Coroutine running;
    int patternIndex;

    void OnEnable()
    {
        if (beatMusic != null)
            beatMusic.OnBeat += HandleBeat;
    }

    void OnDisable()
    {
        if (beatMusic != null)
            beatMusic.OnBeat -= HandleBeat;
    }

    public void ResetPatterns(float beat)
    {
        if (running != null)
            StopCoroutine(running);
        running = null;
        patternIndex = Mathf.Max(0, (int)(beat - introBeats) / 4);
    }

    void HandleBeat(int beat)
    {
        if (beat < introBeats)
            return;
        if ((beat - introBeats) % 4 != 0)
            return;
        if (running != null)
            StopCoroutine(running);
        int level = 1 + Mathf.Max(0, beat - introBeats) / 32;
        running = StartCoroutine(RunPattern(patternIndex % 6, level));
        patternIndex++;
    }

    IEnumerator RunPattern(int index, int level)
    {
        switch (index)
        {
            case 0: yield return AimedShots(level); break;
            case 1: yield return RadialBurst(level); break;
            case 2: yield return BeamCross(level); break;
            case 3: yield return WallSweep(level); break;
            case 4: yield return RingWaves(level); break;
            default: yield return BouncingSquares(level); break;
        }
    }

//aqui va las bolitas que siguen lineas 

    IEnumerator AimedShots(int level)
    {
        int shots = 2 + level;
        for (int i = 0; i < shots; i++)
        {
            Vector2 from = factory.RandomEdgePoint(1.5f);
            Vector2 dir = (Vector2)player.transform.position - from;
            factory.SpawnBullet(from, dir, 9.5f + level * 0.6f);
            yield return WaitBeats(0.5f);
        }
    }

//rafaga circular de balas que salen de un punto random y se dirigen a todas direcciones

    IEnumerator RadialBurst(int level)
    {
        Vector2 center = new Vector2(Random.Range(-4f, 4f), Random.Range(-2f, 2f));
        factory.SpawnTriangleTelegraph(center, beatMusic.SecondsPerBeat);
        yield return WaitBeats(1f);
        int count = 6 + level * 2;
        float spin = Random.Range(0f, 360f);
        for (int i = 0; i < count; i++)
        {
            float a = spin + i * 360f / count + Random.Range(-4f, 4f);
            Vector2 dir = new Vector2(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad));
            factory.SpawnBullet(center + dir * 0.8f, dir, 1.5f);
        }
    }

    IEnumerator BeamCross(int level)
    {
        float spb = beatMusic.SecondsPerBeat;
        Vector2 p = player.transform.position;
        factory.SpawnBeam(new Vector2(0f, p.y), factory.HalfWidth * 2f + 4f, 0.9f, false, spb, spb * 0.75f);
        factory.SpawnBeam(new Vector2(p.x, 0f), factory.HalfHeight * 2f + 4f, 0.9f, true, spb * 1.5f, spb * 0.75f);
        if (level >= 2)
        {
            factory.SpawnBeam(new Vector2(0f, Random.Range(-3f, 3f)), factory.HalfWidth * 2f + 4f, 0.9f, false, spb * 2f, spb * 0.75f);
            factory.SpawnBeam(new Vector2(Random.Range(-6f, 6f), 0f), factory.HalfHeight * 2f + 4f, 0.9f, true, spb * 2f, spb * 0.75f);
        }
        yield return WaitBeats(3.5f);
    }

    IEnumerator WallSweep(int level)
    {
        yield return SweepOnce();
        if (level >= 2)
            yield return SweepOnce();
    }

//el hueco del muro 

    IEnumerator SweepOnce()
    {
        float spb = beatMusic.SecondsPerBeat;
        bool rightward = Random.value < 0.5f;
        float startX = rightward ? -factory.HalfWidth - 1f : factory.HalfWidth + 1f;
        float endX = rightward ? factory.HalfWidth + 1.2f : -factory.HalfWidth - 1.2f;
        float gapCenter = Random.Range(-3f, 3f);
        float gapHalf = 3.6f;
        float top = factory.HalfHeight + 0.6f;
        float bottom = -factory.HalfHeight - 0.6f;

        float topLen = Mathf.Max(0.5f, top - (gapCenter + gapHalf));
        float bottomLen = Mathf.Max(0.5f, (gapCenter - gapHalf) - bottom);

        Beam topBeam = factory.SpawnBeam(
            new Vector2(startX, (top + gapCenter + gapHalf) * 0.5f),
            topLen, 0.8f, true, spb, spb * 4f);
        Beam bottomBeam = factory.SpawnBeam(
            new Vector2(startX, (bottom + gapCenter - gapHalf) * 0.5f),
            bottomLen, 0.8f, true, spb, spb * 4f);

        yield return WaitBeats(1f);

        float travel = spb * 2.5f;
        float t = 0f;
        float yTop = topBeam.transform.position.y;
        float yBottom = bottomBeam.transform.position.y;
        while (t < travel)
        {
            t += Time.deltaTime;
            float x = Mathf.Lerp(startX, endX, t / travel);
            topBeam.transform.position = new Vector3(x, yTop, 0f);
            bottomBeam.transform.position = new Vector3(x, yBottom, 0f);
            yield return null;
        }
    }

    IEnumerator RingWaves(int level)
    {
        float spb = beatMusic.SecondsPerBeat;
        Vector2 center = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(-1f, 1f));
        factory.SpawnRing(center, Random.Range(0f, 360f), spb, 5.5f + level * 0.5f);
        yield return WaitBeats(1.5f);
        factory.SpawnRing(center, Random.Range(0f, 360f), spb, 5.5f + level * 0.5f);
        yield return WaitBeats(3f);
    }

    IEnumerator BouncingSquares(int level)
    {
        float spb = beatMusic.SecondsPerBeat;
        int count = Mathf.Min(4, 1 + level);
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
            Vector2 vel = Random.insideUnitCircle.normalized;
            if (vel.sqrMagnitude < 0.01f)
                vel = Vector2.one.normalized;
            factory.SpawnBouncer(pos, vel * (4f + level * 0.5f), 1.4f, spb * 6f);
            yield return WaitBeats(0.5f);
        }
        yield return WaitBeats(2f);
    }

    IEnumerator WaitBeats(float beats)
    {
        float target = beatMusic.BeatTime + beats;
        while (beatMusic.BeatTime < target)
            yield return null;
    }
}
