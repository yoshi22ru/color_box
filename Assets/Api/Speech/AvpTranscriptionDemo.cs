using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Graffity.Groq.Speech;

public class AvpTranscriptionDemo : MonoBehaviour
{
    [SerializeField] private string _apiKey = "YOUR_API_KEY";
    [SerializeField] private float _recordSeconds = 10f;

    private async void Start()
    {
        var cts = new CancellationTokenSource();
        var recorder = gameObject.AddComponent<AvpRecordingDemo>();

        string wavPath = await recorder.RecordAndSaveAsync(cts.Token);
        if (string.IsNullOrEmpty(wavPath))
        {
            Debug.LogError("録音に失敗しました");
            return;
        }

        var transcription = new Transcription(_apiKey)
            .SetFilePath(wavPath)
            .SetModel(SpeechAiModelType.Whisper_large_v3);

        var result = await transcription.SendAsync(cts.Token);
        Debug.Log("文字起こし結果: " + result.response.Text);
    }
}
