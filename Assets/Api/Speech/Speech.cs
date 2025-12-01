using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Graffity.Groq.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Graffity.Groq.Speech
{
    /// <summary>
    /// TextToSpeech
    /// </summary>
    /// <see cref="https://console.groq.com/docs/api-reference#audio-speech"/>
    public class Speech : IAsyncTextToSpeechRequest<(long statusCode, AudioClip response)>
    {
        private readonly SpeechResponse CanceledResponse = new (){ Text= "Canceled"};
        private readonly string _endpoint = "https://api.groq.com/openai/v1/audio/speech";
        private readonly string _apiKey = string.Empty;
        public Speech(string ApiKey)
        {
            _apiKey = ApiKey;
        }
        #region ===== parameters =====

        [Header("Required")]
        private TTSAiModelType _modelType = TTSAiModelType.Undefined;
        private string _inputText = string.Empty;
        private string _voiceType = string.Empty;

        [Header("Optional")]
        private string _audioFormat = "wav";
        private float _voiceSpeed = 1.0f;

        #endregion //) ===== parameters =====

        #region IAsyncRequest implementation
        public string Endpoint => _endpoint;
        public string ApiKey => _apiKey;

        /// <summary>
        /// API call
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>StatusCode, SpeechResponse</returns>
        public async UniTask<(long statusCode, AudioClip response)> SendAsync(CancellationToken cancellationToken)
        {
            IAsyncRequest<(long statusCode, AudioClip response)> api = this;
            IAsyncTextToSpeechRequest<(long statusCode, AudioClip response)> speechApi = this;
            try
            {
                var jsonData = CreateJsonRequest(speechApi);
                
                // Create UnityWebRequest
                using (UnityWebRequest request = new UnityWebRequest(api.Endpoint, "Post"))
                {
                    // Add Headers
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", "Bearer " + api.ApiKey);

                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();

                    
                    await request.SendWebRequest().WithCancellation(cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Failed to Create Audio file");
                        return (request.responseCode, null);
                    }

                    // Output path
                    string savedFilePath = Path.Combine(Application.persistentDataPath, "response.wav");
                    // Fix Wav Header for streaming to local file
                    WavFileWriter.FixWavHeader(request.downloadHandler.data, savedFilePath, 44100,1);
                    
                    // Read wav data as AudioClip
                    var clip = await LoadWavFileAsync(savedFilePath, cancellationToken);
                    return (request.responseCode, clip);
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning(e);
                return (408, null);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private string CreateJsonRequest(IAsyncTextToSpeechRequest<(long statusCode, AudioClip response)> speechApi)
        {
            var requestBody = new RequestBody();
            // Required
            requestBody.model=speechApi.ModelType.ToWWWField();
            requestBody.input=speechApi.Text;
            requestBody.voice=speechApi.Voice;
            // Optional
            requestBody.response_format= speechApi.AudioFormat;
            requestBody.speed=speechApi.Speed;
            
            return JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            });
        }
        private async UniTask<AudioClip> LoadWavFileAsync(string path, CancellationToken cancellationToken = default)
        {
            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV))
            {
                await request.SendWebRequest().WithCancellation(cancellationToken);

                if (request.result != UnityWebRequest.Result.Success)
                    throw new System.Exception("Failed to load wav: " + request.error);

                return DownloadHandlerAudioClip.GetContent(request);
            }
        }
        

        [System.Serializable]
        public class RequestBody
        {
            public string model;
            public string input;
            public string voice;
            public string response_format;
            public float speed;
        }

        #endregion //) IAsyncRequest implementation

        #region ===== IAsyncTextToSpeechRequest =====


        /// <summary>
        /// AI Model Type
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="modelType"></param>
        /// <returns>this</returns>
        public IAsyncTextToSpeechRequest<(long statusCode, AudioClip response)> SetModel(TTSAiModelType modelType)
        {
            _modelType = modelType;
            return this;
        }


        public TTSAiModelType ModelType => _modelType;

        /// <summary>
        /// The language of the input audio.
        /// Supplying the input language in ISO-639-1 format will improve accuracy and latency.
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="input"></param>
        /// <returns>this</returns>
        public IAsyncTextToSpeechRequest<(long statusCode, AudioClip response)> SetText(string input)
        {
            _inputText = input;
            return this;
        }
        public string Text => _inputText;

        /// <summary>
        /// VoiceType
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="voice">Use TTSEnglishVoiceType/TTSArbicVoiceType toString()</param>
        /// <returns>this</returns>
        public IAsyncTextToSpeechRequest<(long statusCode, AudioClip response)> SetVoice(string voice)
        {
            _voiceType = voice;
            return this;
        }

        public string Voice => _voiceType;

        /// <summary>
        /// Audio Format 
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="format">default: wav</param>
        /// <returns>this</returns>
        public IAsyncTextToSpeechRequest<(long statusCode, AudioClip response)> SetAudioFormat(string format)
        {
            _audioFormat = format;
            return this;
        }

        public string AudioFormat => _audioFormat;

        /// <summary>
        /// The speed of the generated audio. 1.0 is the only supported value.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="speed">default: 1.0</param>
        /// <returns>this</returns>
        public IAsyncTextToSpeechRequest<(long statusCode, AudioClip response)> SetSpeed(float speed)
        {
            _voiceSpeed = speed;
            return this;
        }

        public float Speed => _voiceSpeed;

        #endregion //) ===== IAsyncTextToSpeechRequest =====

    }
    
}
