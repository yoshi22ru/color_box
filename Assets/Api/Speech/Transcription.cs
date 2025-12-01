using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Graffity.Groq.Speech
{
    public class Transcription
    {
        private readonly string _endpoint = "https://api.groq.com/openai/v1/audio/transcriptions";
        private readonly string _apiKey;
        private string _audioFilePath;
        private SpeechAiModelType _modelType = SpeechAiModelType.Whisper_large_v3;

        public Transcription(string apiKey)
        {
            _apiKey = apiKey;
        }

        public Transcription SetFilePath(string path)
        {
            _audioFilePath = path;
            return this;
        }

        public Transcription SetModel(SpeechAiModelType model)
        {
            _modelType = model;
            return this;
        }

       
        public async UniTask<(long statusCode, SpeechResponse response)> SendAsync(CancellationToken token)
        {
            try
            {
                byte[] fileData = global::System.IO.File.ReadAllBytes(_audioFilePath); // 修正
                WWWForm form = new WWWForm();
                form.AddBinaryData("file", fileData, Path.GetFileName(_audioFilePath), "application/octet-stream");
                form.AddField("model", _modelType.ToModelID());

                using var request = UnityWebRequest.Post(_endpoint, form);
                request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

                await request.SendWebRequest().WithCancellation(token);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var res = JsonConvert.DeserializeObject<SpeechResponse>(request.downloadHandler.text);
                    return (request.responseCode, res);
                }
                else
                {
                    Debug.LogError("Upload Failed: " + request.error);
                    return (request.responseCode, new SpeechResponse() { Text = request.error });
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

    }
}
