using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates procedural audio clips at runtime for game sounds.
/// Creates all necessary sound effects without external audio files.
/// </summary>
public static class ProceduralAudio
{
    private static Dictionary<string, AudioClip> cachedClips = new Dictionary<string, AudioClip>();
    private static bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (initialized) return;

        GenerateAllSounds();
        initialized = true;
        Debug.Log("[ProceduralAudio] Generated all procedural sound effects");
    }

    private static void GenerateAllSounds()
    {
        // Player sounds
        cachedClips["PlayerAttack1"] = GenerateSwishSound(0.15f, 800f, 400f);
        cachedClips["PlayerAttack2"] = GenerateSwishSound(0.15f, 900f, 350f);
        cachedClips["PlayerAttack3"] = GenerateSwishSound(0.2f, 1000f, 300f);
        cachedClips["PlayerHit"] = GenerateImpactSound(0.2f, 200f, 0.8f);
        cachedClips["PlayerDodge"] = GenerateWhooshSound(0.25f, 600f, 200f);
        cachedClips["PlayerHurt"] = GenerateHurtSound(0.3f, 300f);
        cachedClips["PlayerDeath"] = GenerateDeathSound(0.8f, 200f);

        // Boss sounds
        cachedClips["BossRoar"] = GenerateRoarSound(0.6f, 120f);
        cachedClips["BossSweep"] = GenerateSwishSound(0.3f, 400f, 150f);
        cachedClips["BossSlam"] = GenerateImpactSound(0.4f, 80f, 1f);
        cachedClips["BossProjectile"] = GenerateProjectileSound(0.3f, 500f);
        cachedClips["BossHurt"] = GenerateImpactSound(0.25f, 150f, 0.7f);
        cachedClips["BossPhaseTransition"] = GeneratePhaseTransitionSound(1.2f);
        cachedClips["BossDeath"] = GenerateExplosionSound(1.5f);

        // Combat sounds
        cachedClips["Parry"] = GenerateParrySound(0.3f);
        cachedClips["CriticalHit"] = GenerateCriticalSound(0.4f);
        cachedClips["ComboFinish"] = GenerateComboSound(0.5f);

        // UI sounds
        cachedClips["UIClick"] = GenerateClickSound(0.1f);
        cachedClips["UIHover"] = GenerateHoverSound(0.05f);
        cachedClips["UIConfirm"] = GenerateConfirmSound(0.2f);
        cachedClips["UICancel"] = GenerateCancelSound(0.15f);

        // Game state sounds
        cachedClips["Victory"] = GenerateVictorySound(2f);
        cachedClips["Defeat"] = GenerateDefeatSound(1.5f);
        cachedClips["MenuMusic"] = GenerateMenuMusicLoop(8f);
        cachedClips["BattleMusic"] = GenerateBattleMusicLoop(8f);
    }

    #region Sound Generation Methods

    private static AudioClip GenerateSwishSound(float duration, float startFreq, float endFreq)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = Mathf.Lerp(startFreq, endFreq, t);
            float amplitude = Mathf.Sin(t * Mathf.PI) * 0.5f; // Fade in/out
            float noise = (Random.value * 2 - 1) * 0.3f;

            data[i] = (Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate) * 0.5f + noise) * amplitude;
        }

        return CreateClip("Swish", data, sampleRate);
    }

    private static AudioClip GenerateImpactSound(float duration, float freq, float intensity)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 10f) * intensity;
            float noise = (Random.value * 2 - 1);

            // Low thump + noise burst
            float thump = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            data[i] = (thump * 0.6f + noise * 0.4f) * envelope;
        }

        return CreateClip("Impact", data, sampleRate);
    }

    private static AudioClip GenerateWhooshSound(float duration, float startFreq, float endFreq)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = Mathf.Lerp(startFreq, endFreq, t * t); // Exponential frequency drop
            float amplitude = Mathf.Sin(t * Mathf.PI) * 0.4f;

            // Filtered noise
            float noise = Mathf.PerlinNoise(i * 0.01f, 0) * 2 - 1;
            data[i] = noise * amplitude * Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate * 0.1f);
        }

        return CreateClip("Whoosh", data, sampleRate);
    }

    private static AudioClip GenerateHurtSound(float duration, float baseFreq)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 5f);

            // Descending tone with vibrato
            float vibrato = Mathf.Sin(t * 50f) * 30f;
            float freq = baseFreq * (1 - t * 0.3f) + vibrato;
            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);

            data[i] = tone * envelope * 0.6f;
        }

        return CreateClip("Hurt", data, sampleRate);
    }

    private static AudioClip GenerateDeathSound(float duration, float baseFreq)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 3f);

            // Dramatic descending tone
            float freq = baseFreq * Mathf.Exp(-t * 2f);
            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            float noise = (Random.value * 2 - 1) * 0.2f;

            data[i] = (tone * 0.7f + noise) * envelope;
        }

        return CreateClip("Death", data, sampleRate);
    }

    private static AudioClip GenerateRoarSound(float duration, float baseFreq)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;

            // Build up then decay
            float envelope;
            if (t < 0.2f)
                envelope = t / 0.2f;
            else
                envelope = Mathf.Exp(-(t - 0.2f) * 3f);

            // Multiple harmonics + distortion
            float freq1 = baseFreq * (1 + Mathf.Sin(t * 10f) * 0.1f);
            float freq2 = baseFreq * 2.01f;
            float freq3 = baseFreq * 3.02f;

            float tone = Mathf.Sin(2 * Mathf.PI * freq1 * i / sampleRate) * 0.5f +
                        Mathf.Sin(2 * Mathf.PI * freq2 * i / sampleRate) * 0.3f +
                        Mathf.Sin(2 * Mathf.PI * freq3 * i / sampleRate) * 0.2f;

            // Add distortion
            tone = Mathf.Sign(tone) * Mathf.Pow(Mathf.Abs(tone), 0.7f);

            float noise = (Random.value * 2 - 1) * 0.15f;
            data[i] = (tone + noise) * envelope * 0.8f;
        }

        return CreateClip("Roar", data, sampleRate);
    }

    private static AudioClip GenerateProjectileSound(float duration, float freq)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 8f);

            // Pew sound - quick frequency sweep
            float sweepFreq = freq * (1 + (1 - t) * 2f);
            float tone = Mathf.Sin(2 * Mathf.PI * sweepFreq * i / sampleRate);

            data[i] = tone * envelope * 0.5f;
        }

        return CreateClip("Projectile", data, sampleRate);
    }

    private static AudioClip GenerateParrySound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 12f);

            // Metallic ring with harmonics
            float f1 = 1200f;
            float f2 = 2400f;
            float f3 = 3600f;

            float tone = Mathf.Sin(2 * Mathf.PI * f1 * i / sampleRate) * 0.5f +
                        Mathf.Sin(2 * Mathf.PI * f2 * i / sampleRate) * 0.3f +
                        Mathf.Sin(2 * Mathf.PI * f3 * i / sampleRate) * 0.2f;

            data[i] = tone * envelope * 0.6f;
        }

        return CreateClip("Parry", data, sampleRate);
    }

    private static AudioClip GenerateCriticalSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 8f);

            // Rising then falling pitch
            float pitchMod = t < 0.3f ? t / 0.3f : 1 - (t - 0.3f) / 0.7f;
            float freq = 600f + pitchMod * 600f;

            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            float sparkle = Mathf.Sin(2 * Mathf.PI * freq * 3f * i / sampleRate) * 0.3f;

            data[i] = (tone * 0.6f + sparkle) * envelope;
        }

        return CreateClip("Critical", data, sampleRate);
    }

    private static AudioClip GenerateComboSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Sin(t * Mathf.PI);

            // Ascending arpeggio
            float noteT = (t * 4) % 1f;
            int noteIndex = (int)(t * 4) % 4;
            float[] notes = { 523f, 659f, 784f, 1047f }; // C5, E5, G5, C6
            float freq = notes[noteIndex];

            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);

            data[i] = tone * envelope * Mathf.Exp(-noteT * 5f) * 0.5f;
        }

        return CreateClip("Combo", data, sampleRate);
    }

    private static AudioClip GeneratePhaseTransitionSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;

            // Build up to climax
            float envelope;
            if (t < 0.7f)
                envelope = t / 0.7f;
            else
                envelope = 1 - (t - 0.7f) / 0.3f;

            // Rising frequency with rumble
            float freq = 80f + t * 400f;
            float rumble = Mathf.Sin(2 * Mathf.PI * 40f * i / sampleRate) * 0.3f;
            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            float noise = (Random.value * 2 - 1) * 0.2f * t;

            data[i] = (tone * 0.5f + rumble + noise) * envelope * 0.7f;
        }

        return CreateClip("PhaseTransition", data, sampleRate);
    }

    private static AudioClip GenerateExplosionSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;

            // Initial burst then decay
            float envelope;
            if (t < 0.05f)
                envelope = t / 0.05f;
            else
                envelope = Mathf.Exp(-(t - 0.05f) * 3f);

            // Low rumble + noise burst
            float rumble = Mathf.Sin(2 * Mathf.PI * 50f * i / sampleRate) * 0.4f;
            float rumble2 = Mathf.Sin(2 * Mathf.PI * 30f * i / sampleRate) * 0.3f;
            float noise = (Random.value * 2 - 1) * (1 - t);

            data[i] = (rumble + rumble2 + noise * 0.5f) * envelope * 0.8f;
        }

        return CreateClip("Explosion", data, sampleRate);
    }

    private static AudioClip GenerateClickSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 40f);
            float tone = Mathf.Sin(2 * Mathf.PI * 1000f * i / sampleRate);
            data[i] = tone * envelope * 0.4f;
        }

        return CreateClip("Click", data, sampleRate);
    }

    private static AudioClip GenerateHoverSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Sin(t * Mathf.PI);
            float tone = Mathf.Sin(2 * Mathf.PI * 800f * i / sampleRate);
            data[i] = tone * envelope * 0.2f;
        }

        return CreateClip("Hover", data, sampleRate);
    }

    private static AudioClip GenerateConfirmSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 10f);

            // Two-note confirm (low then high)
            float freq = t < 0.5f ? 523f : 784f; // C5 then G5
            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            data[i] = tone * envelope * 0.4f;
        }

        return CreateClip("Confirm", data, sampleRate);
    }

    private static AudioClip GenerateCancelSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float envelope = Mathf.Exp(-t * 15f);

            // Descending tone
            float freq = 600f * (1 - t * 0.5f);
            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            data[i] = tone * envelope * 0.4f;
        }

        return CreateClip("Cancel", data, sampleRate);
    }

    private static AudioClip GenerateVictorySound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        float[] melody = { 523f, 659f, 784f, 1047f, 784f, 1047f }; // Victory fanfare
        float noteDuration = duration / melody.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            int noteIndex = Mathf.Min((int)(t * melody.Length), melody.Length - 1);
            float noteT = (t * melody.Length) % 1f;

            float freq = melody[noteIndex];
            float envelope = Mathf.Exp(-noteT * 3f);
            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            float harmonic = Mathf.Sin(2 * Mathf.PI * freq * 2 * i / sampleRate) * 0.3f;

            data[i] = (tone + harmonic) * envelope * 0.5f;
        }

        return CreateClip("Victory", data, sampleRate);
    }

    private static AudioClip GenerateDefeatSound(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;

            // Sad descending tone
            float freq = 400f * Mathf.Exp(-t * 1.5f);
            float envelope = 1 - t;
            float tone = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            float minor = Mathf.Sin(2 * Mathf.PI * freq * 1.2f * i / sampleRate) * 0.3f; // Minor third

            data[i] = (tone * 0.6f + minor) * envelope * 0.5f;
        }

        return CreateClip("Defeat", data, sampleRate);
    }

    private static AudioClip GenerateMenuMusicLoop(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        // Simple ambient pad
        float[] chordFreqs = { 261f, 329f, 392f }; // C major chord

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float sum = 0;

            foreach (float baseFreq in chordFreqs)
            {
                float lfo = 1 + Mathf.Sin(t * Mathf.PI * 2 * 0.5f) * 0.02f;
                float freq = baseFreq * lfo;
                sum += Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate);
            }

            // Soft attack/release for looping
            float envelope = 1f;
            if (t < 0.1f) envelope = t / 0.1f;
            else if (t > 0.9f) envelope = (1 - t) / 0.1f;

            data[i] = sum / chordFreqs.Length * 0.3f * envelope;
        }

        return CreateClip("MenuMusic", data, sampleRate);
    }

    private static AudioClip GenerateBattleMusicLoop(float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        float bpm = 150f;
        float beatLength = 60f / bpm;
        int samplesPerBeat = (int)(beatLength * sampleRate);

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            int beatIndex = i / samplesPerBeat;
            float beatT = (float)(i % samplesPerBeat) / samplesPerBeat;

            float sum = 0;

            // Kick drum on 1 and 3
            if (beatIndex % 2 == 0)
            {
                float kickEnv = Mathf.Exp(-beatT * 15f);
                float kickFreq = 60f * (1 + (1 - beatT) * 2f);
                sum += Mathf.Sin(2 * Mathf.PI * kickFreq * beatT) * kickEnv * 0.4f;
            }

            // Snare on 2 and 4
            if (beatIndex % 2 == 1)
            {
                float snareEnv = Mathf.Exp(-beatT * 20f);
                sum += (Random.value * 2 - 1) * snareEnv * 0.3f;
            }

            // Bass line
            float[] bassNotes = { 65f, 65f, 82f, 73f }; // E2, E2, A2, F#2
            float bassFreq = bassNotes[beatIndex % 4];
            float bassEnv = Mathf.Exp(-beatT * 5f);
            sum += Mathf.Sin(2 * Mathf.PI * bassFreq * i / sampleRate) * bassEnv * 0.35f;

            // Looping envelope
            float envelope = 1f;
            if (t < 0.05f) envelope = t / 0.05f;
            else if (t > 0.95f) envelope = (1 - t) / 0.05f;

            data[i] = Mathf.Clamp(sum * envelope, -1f, 1f);
        }

        return CreateClip("BattleMusic", data, sampleRate);
    }

    #endregion

    #region Helper Methods

    private static AudioClip CreateClip(string name, float[] data, int sampleRate)
    {
        AudioClip clip = AudioClip.Create(name, data.Length, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    #endregion

    #region Public Access

    public static AudioClip GetClip(string name)
    {
        if (!initialized) Initialize();

        if (cachedClips.TryGetValue(name, out AudioClip clip))
            return clip;

        Debug.LogWarning($"[ProceduralAudio] Clip '{name}' not found");
        return null;
    }

    public static AudioClip GetPlayerAttackSound(int comboIndex = 1)
    {
        return GetClip($"PlayerAttack{Mathf.Clamp(comboIndex, 1, 3)}");
    }

    public static AudioClip GetPlayerHitSound() => GetClip("PlayerHit");
    public static AudioClip GetPlayerDodgeSound() => GetClip("PlayerDodge");
    public static AudioClip GetPlayerHurtSound() => GetClip("PlayerHurt");
    public static AudioClip GetPlayerDeathSound() => GetClip("PlayerDeath");

    public static AudioClip GetBossRoarSound() => GetClip("BossRoar");
    public static AudioClip GetBossSweepSound() => GetClip("BossSweep");
    public static AudioClip GetBossSlamSound() => GetClip("BossSlam");
    public static AudioClip GetBossProjectileSound() => GetClip("BossProjectile");
    public static AudioClip GetBossHurtSound() => GetClip("BossHurt");
    public static AudioClip GetBossPhaseTransitionSound() => GetClip("BossPhaseTransition");
    public static AudioClip GetBossDeathSound() => GetClip("BossDeath");

    public static AudioClip GetParrySound() => GetClip("Parry");
    public static AudioClip GetCriticalSound() => GetClip("CriticalHit");
    public static AudioClip GetComboSound() => GetClip("ComboFinish");

    public static AudioClip GetUIClickSound() => GetClip("UIClick");
    public static AudioClip GetUIHoverSound() => GetClip("UIHover");
    public static AudioClip GetUIConfirmSound() => GetClip("UIConfirm");
    public static AudioClip GetUICancelSound() => GetClip("UICancel");

    public static AudioClip GetVictorySound() => GetClip("Victory");
    public static AudioClip GetDefeatSound() => GetClip("Defeat");
    public static AudioClip GetMenuMusic() => GetClip("MenuMusic");
    public static AudioClip GetBattleMusic() => GetClip("BattleMusic");

    #endregion
}
