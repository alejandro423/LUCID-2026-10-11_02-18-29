using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeatMusic : MonoBehaviour
{
    public int bpm = 128;
    public int loopBeats = 8;

    public float SecondsPerBeat => 60f / bpm;
    public float BeatTime { get; private set; } = -1f;
    public int BeatIndex => Mathf.Max(0, Mathf.FloorToInt(BeatTime));
    public event Action<int> OnBeat;

    AudioSource source;
    double dspStart;
    int lastBeat = -1;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = BuildLoop();
        source.loop = true;
        source.playOnAwake = false;
        double delay = 0.5;
        source.PlayScheduled(AudioSettings.dspTime + delay);
        dspStart = AudioSettings.dspTime + delay;
    }

    void Update()
    {
        BeatTime = (float)((AudioSettings.dspTime - dspStart) / SecondsPerBeat);
        int beat = Mathf.FloorToInt(BeatTime);
        if (beat >= 0 && beat != lastBeat)
        {
            lastBeat = beat;
            OnBeat?.Invoke(beat);
        }
    }

    public void RestartAt(float beat)
    {
        source.Stop();
        double delay = 0.5;
        double start = AudioSettings.dspTime + delay;
        source.PlayScheduled(start);
        dspStart = start - beat * SecondsPerBeat;
        lastBeat = Mathf.FloorToInt(beat) - 1;
    }

    AudioClip BuildLoop()
    {
        int sampleRate = AudioSettings.outputSampleRate;
        float spb = SecondsPerBeat;
        int length = Mathf.RoundToInt(spb * loopBeats * sampleRate);
        float[] data = new float[length];
        float[] kick = RenderKick(0.3f, sampleRate);
        float[] hat = RenderNoise(0.05f, 60f, sampleRate);
        float[] snare = RenderNoise(0.14f, 22f, sampleRate);

        for (int b = 0; b < loopBeats; b++)
        {
            Mix(data, kick, Mathf.RoundToInt(b * spb * sampleRate), 0.9f);
            if (b % 2 == 1)
                Mix(data, snare, Mathf.RoundToInt(b * spb * sampleRate), 0.45f);
            Mix(data, hat, Mathf.RoundToInt(b * spb * sampleRate), 0.18f);
            Mix(data, hat, Mathf.RoundToInt((b + 0.5f) * spb * sampleRate), 0.3f);
        }

        float[] bassLine = { 55f, 0f, 55f, 65.41f, 55f, 0f, 98f, 82.41f, 55f, 0f, 55f, 65.41f, 82.41f, 73.42f, 65.41f, 55f };
        for (int i = 0; i < bassLine.Length; i++)
        {
            if (bassLine[i] <= 0f)
                continue;
            float[] note = RenderBassNote(bassLine[i], spb * 0.45f, sampleRate);
            Mix(data, note, Mathf.RoundToInt(i * spb * 0.5f * sampleRate), 0.4f);
        }

        for (int i = 0; i < data.Length; i++)
            data[i] *= 0.8f;

        AudioClip clip = AudioClip.Create("beat_loop", length, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    float[] RenderKick(float duration, int sampleRate)
    {
        int n = Mathf.RoundToInt(duration * sampleRate);
        float[] s = new float[n];
        float phase = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / sampleRate;
            float freq = Mathf.Lerp(160f, 45f, Mathf.Clamp01(t / 0.1f));
            phase += 2f * Mathf.PI * freq / sampleRate;
            s[i] = Mathf.Sin(phase) * Mathf.Exp(-t * 9f);
        }
        return s;
    }

    float[] RenderNoise(float duration, float decay, int sampleRate)
    {
        int n = Mathf.RoundToInt(duration * sampleRate);
        float[] s = new float[n];
        var rng = new System.Random(12345);
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / sampleRate;
            float noise = (float)(rng.NextDouble() * 2.0 - 1.0);
            s[i] = noise * Mathf.Exp(-t * decay);
        }
        return s;
    }

    float[] RenderBassNote(float freq, float duration, int sampleRate)
    {
        int n = Mathf.RoundToInt(duration * sampleRate);
        float[] s = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / sampleRate;
            float wave = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t));
            float env = Mathf.Exp(-t * 5f) * 0.7f + 0.3f * Mathf.Exp(-t * 20f);
            s[i] = wave * env * 0.35f;
        }
        return s;
    }

    void Mix(float[] buffer, float[] sample, int offset, float gain)
    {
        for (int i = 0; i < sample.Length; i++)
        {
            int idx = offset + i;
            if (idx < 0 || idx >= buffer.Length)
                continue;
            buffer[idx] += sample[i] * gain;
        }
    }
}
