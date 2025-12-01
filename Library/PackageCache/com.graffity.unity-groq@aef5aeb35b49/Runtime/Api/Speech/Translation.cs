using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Graffity.Groq.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Graffity.Groq.Speech
{
    /// <summary>
    /// Translation audio file to english text
    /// </summary>
    public class Translation : IAsyncSpeechRequest<(long statusCode, SpeechResponse response)>
    {
        private readonly SpeechResponse CanceledResponse = new (){ Text= "Canceled"};
        private readonly string _endpoint = "https://api.groq.com/openai/v1/audio/translations";
        private readonly string _apiKey = string.Empty;
        public Translation(string ApiKey)
        {
            _apiKey = ApiKey;
        }
        #region ===== parameters =====

        [Header("Required")]
        private string _audioFilePath = string.Empty;
        private SpeechAiModelType _modelType = SpeechAiModelType.Undefined;

        [Header("Optional")]
        private string _language = string.Empty;
        private string _promptText = string.Empty;
        private float _temperture = 0f;

        #endregion //) ===== parameters =====

        #region IAsyncRequest implementation
        public string Endpoint => _endpoint;
        public string ApiKey => _apiKey;

        /// <summary>
        /// API call
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>StatusCode, SpeechResponse</returns>
        public async UniTask<(long statusCode, SpeechResponse response)> SendAsync(CancellationToken cancellationToken)
        {
            IAsyncRequest<(long statusCode, SpeechResponse response)> api = this;
            IAsyncSpeechRequest<(long statusCode, SpeechResponse response)> speechApi = this;
            try
            {
                // ファイルをバイト配列として読み込む
                byte[] fileData = System.IO.File.ReadAllBytes(speechApi.AudioFilePath);
                
                // フォームデータを作成
                WWWForm form = new WWWForm();
                // Required
                form.AddBinaryData("file", fileData, System.IO.Path.GetFileName(speechApi.AudioFilePath), "application/octet-stream");
                form.AddField("model",speechApi.ModelType.ToModelID());
                // Optional
                if(!string.IsNullOrEmpty(speechApi.PromptText))form.AddField("prompt",speechApi.PromptText);
                form.AddField("temperature",speechApi.Temperture.ToString("0.00"));

                // UnityWebRequest を作成
                using(UnityWebRequest request = UnityWebRequest.Post(api.Endpoint, form))
                {
                    // Authorization ヘッダーを追加
                    request.SetRequestHeader("Authorization", "Bearer " + api.ApiKey);

                    await request.SendWebRequest().WithCancellation(cancellationToken);

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Upload Success: " + request.downloadHandler.text);
                        var res = JsonUtility.FromJson<SpeechResponse>(request.downloadHandler.text);
                        return (request.responseCode, res);
                    }
                    else
                    {
                        Debug.LogError("Upload Failed: " + request.error);
                        var res = new SpeechResponse() { Text = request.error };
                        return (request.responseCode, res);
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning(e);
                return (408, CanceledResponse);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
        #endregion //) IAsyncRequest implementation

        #region ===== IAsyncSpeechRequest =====

        
        /// <summary>
        /// Audio File Path
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="filePath"></param>
        /// <returns>this</returns>
        public IAsyncSpeechRequest<(long statusCode, SpeechResponse response)> SetFilePath(string filePath)
        {
            _audioFilePath = filePath;
            return this;
        }

        public string AudioFilePath => _audioFilePath;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="modelType"></param>
        /// <returns>this</returns>
        public IAsyncSpeechRequest<(long statusCode, SpeechResponse response)> SetModel(SpeechAiModelType modelType)
        {
            _modelType = modelType;
            return this;
        }
        public SpeechAiModelType ModelType => _modelType;
        
        /// <summary>
        /// The language of the input audio.
        /// Supplying the input language in ISO-639-1 format will improve accuracy and latency.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="language"></param>
        /// <returns>this</returns>
        public IAsyncSpeechRequest<(long statusCode, SpeechResponse response)> SetLanguage(string language)
        {
            _language = language;
            return this;
        }

        public string Language => _language;

        /// <summary>
        /// Prompt text
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="prompt"></param>
        /// <returns>this</returns>
        public IAsyncSpeechRequest<(long statusCode, SpeechResponse response)> SetPrompt(string prompt)
        {
            _promptText = prompt;
            return this;
        }
        public string PromptText => _promptText;

        /// <summary>
        /// PThe sampling temperature, between 0 and 1.
        /// Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// If set to 0, the model will use log probability to automatically increase the temperature until certain thresholds are hit.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[0 1]</param>
        /// <returns>this</returns>
        public IAsyncSpeechRequest<(long statusCode, SpeechResponse response)> SetTemperature(float value)
        {
            _temperture = Mathf.Clamp01(value);
            return this;
        }

        public float Temperture => _temperture;

        #endregion //) ===== IAsyncSpeechRequest =====

    }
    
}
