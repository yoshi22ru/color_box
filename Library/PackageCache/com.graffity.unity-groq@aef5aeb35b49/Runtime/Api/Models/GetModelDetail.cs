using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Graffity.Groq.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Graffity.Groq.Models
{
    /// <summary>
    /// Get Model Detail
    /// </summary>
    /// <see cref="https://console.groq.com/docs/api-reference#models-retrieve"/>
    public class ModelDetailApi : IAsyncModelRequest<(long statusCode, ModelResponse response)>
    {

        private readonly string _endpoint = "https://api.groq.com/openai/v1/models";
        private readonly string _apiKey = string.Empty;
        public ModelDetailApi(string ApiKey)
        {
            _apiKey = ApiKey;
        }

        private string _modelName = string.Empty;
        
        #region IAsyncRequest implementation

        public string Endpoint => _endpoint;
        public string ApiKey => _apiKey;

        /// <summary>
        /// API call
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>StatusCode, ModelResponse</returns>
        public async UniTask<(long statusCode, ModelResponse response)> SendAsync(CancellationToken cancellationToken)
        {
            IAsyncModelRequest<(long statusCode, ModelResponse response)> api = this;
            try
            {
                string endpoint = $"{api.Endpoint}/{api.ModelName}";
                // UnityWebRequest を作成
                using (UnityWebRequest request = new UnityWebRequest(endpoint, "Get"))
                {
                    // Authorization ヘッダーを追加
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", "Bearer " + api.ApiKey);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    
                    await request.SendWebRequest().WithCancellation(cancellationToken);

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var res = JsonConvert.DeserializeObject<ModelResponse>(request.downloadHandler.text);
                        return (request.responseCode, res);
                    }
                    else
                    {
                        return (request.responseCode, null);
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning(e.ToString());
                return (408, null);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
        }
        
        #endregion //) IAsyncRequest implementation
        
        #region ===== IAsyncModelRequest =====
        /// <summary>
        /// for REST Endpoint 
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns>this</returns>
        public IAsyncModelRequest<(long statusCode, ModelResponse response)> SetModelName(string modelName)
        {
            _modelName = modelName;
            return this;
        }

        public string ModelName => _modelName;

        #endregion //) ===== IAsyncModelRequest =====
    }
}

