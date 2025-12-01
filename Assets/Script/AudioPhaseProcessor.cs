using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPhaseProcessor : MonoBehaviour
{
    [Tooltip("Maximum allowed delay in milliseconds (absolute). Must be > max used by MetalSurfaceAudio.")]
    public float maxDelayMs = 20f;

    float[] monoBuffer;
    int bufferPos = 0;
    int sampleRate;

    volatile float delaySeconds = 0f;

    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
        int bufferSize = Mathf.CeilToInt(sampleRate * (maxDelayMs / 1000f + 0.1f));
        monoBuffer = new float[Mathf.Max(512, bufferSize)];
    }

    public void SetInterauralDelayMs(float ms)
    {
        delaySeconds = ms * 0.001f;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (channels < 2) return; 

        int frames = data.Length / channels;

        for (int n = 0; n < frames; n++)
        {
            int idx = n * channels;
            
            float inL = data[idx];
            float inR = data[idx + 1];
            float mono = 0.5f * (inL + inR);

            monoBuffer[bufferPos] = mono;

            float dSec = delaySeconds;
            float delaySamplesF = dSec * sampleRate;
            float readPosF = bufferPos - delaySamplesF;
            
            while (readPosF < 0) readPosF += monoBuffer.Length;
            while (readPosF >= monoBuffer.Length) readPosF -= monoBuffer.Length;

            int i0 = (int)Mathf.Floor(readPosF);
            int i1 = (i0 + 1) % monoBuffer.Length;
            float frac = readPosF - i0;
            float delayedMono = Mathf.Lerp(monoBuffer[i0], monoBuffer[i1], frac);

            float outL = inL; 
            float outR = Mathf.Lerp(inR, delayedMono, 0.95f);

            data[idx] = outL;
            data[idx + 1] = outR;

            bufferPos++;
            if (bufferPos >= monoBuffer.Length) bufferPos = 0;
        }
    }
}
