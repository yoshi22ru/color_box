using System;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AvpRecordingDemo : MonoBehaviour
{
    private const int SampleRate = 48000;
    public const string DefaultWavName = "avp_record.wav";
    private const int BitsPerSample = 16;
    private const int ChannelCount = 1;

    private const int BytesPerSample = BitsPerSample / 8;
    private const float Pcm16MaxAmplitude = 32767f;
    private const int RiffHeaderSize = 36;
    private const int Subchunk1Size = 16;
    private const int AudioFormatPcm = 1;

    [SerializeField]
    private float _recordSeconds = 10f;

    public async UniTask<string> RecordAndSaveAsync(CancellationToken token)
    {
        var device = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;
        if (string.IsNullOrEmpty(device))
        {
            Debug.LogError("マイクが見つかりません");
            return null;
        }

        try
        {
            var audioClip = Microphone.Start(device, false, Mathf.CeilToInt(_recordSeconds), SampleRate);
            Debug.Log("録音開始...");
            await UniTask.WaitUntil(() => !Microphone.IsRecording(device), cancellationToken: token);
            Debug.Log("録音完了");

            var wavBytes = ConvertToWavByteArray(audioClip);
            var savedPath = Path.Combine(Application.persistentDataPath, DefaultWavName);
            await File.WriteAllBytesAsync(savedPath, wavBytes, cancellationToken: token);
            Debug.Log("WAV 保存完了: " + savedPath);
            return savedPath;
        }
        catch (Exception e)
        {
            Debug.LogError($"WAV保存失敗: {e.Message}");
            return null;
        }
    }

    private byte[] ConvertToWavByteArray(AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        var pcm = new byte[samples.Length * BytesPerSample];
        for (int i = 0; i < samples.Length; i++)
        {
            short s = (short)Mathf.Clamp(samples[i] * Pcm16MaxAmplitude, short.MinValue, short.MaxValue);
            pcm[i * 2] = (byte)(s & 0xFF);
            pcm[i * 2 + 1] = (byte)((s >> 8) & 0xFF);
        }

        using var ms = new MemoryStream();
        WriteWavHeader(ms, ChannelCount, SampleRate, pcm.Length);
        ms.Write(pcm, 0, pcm.Length);
        return ms.ToArray();
    }

    private void WriteWavHeader(Stream stream, int channels, int sampleRate, int dataLength)
    {
        int byteRate = sampleRate * channels * BytesPerSample;
        int blockAlign = channels * BytesPerSample;

        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.Write(Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(RiffHeaderSize + dataLength);
        writer.Write(Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(Encoding.UTF8.GetBytes("fmt "));
        writer.Write(Subchunk1Size);
        writer.Write((short)AudioFormatPcm);
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write((short)blockAlign);
        writer.Write((short)BitsPerSample);
        writer.Write(Encoding.UTF8.GetBytes("data"));
        writer.Write(dataLength);
    }
}
