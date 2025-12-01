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
    /// Get Model List
    /// </summary>
    /// <see cref="https://console.groq.com/docs/api-reference#models-list"/>
    public class ModelListApi : IAsyncRequest<(long statusCode, ModelListResponse response)>
    {

        private readonly string _endpoint = "https://api.groq.com/openai/v1/models";
        private readonly string _apiKey = string.Empty;
        public ModelListApi(string ApiKey)
        {
            _apiKey = ApiKey;
        }

        #region IAsyncRequest implementation

        public string Endpoint => _endpoint;
        public string ApiKey => _apiKey;

        /// <summary>
        /// API call
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>StatusCode, ModelListResponse</returns>
        public async UniTask<(long statusCode, ModelListResponse response)> SendAsync(CancellationToken cancellationToken)
        {
            IAsyncRequest<(long statusCode, ModelListResponse response)> api = this;
            try
            {
                // UnityWebRequest を作成
                using (UnityWebRequest request = new UnityWebRequest(api.Endpoint, "Get"))
                {
                    // Authorization ヘッダーを追加
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", "Bearer " + api.ApiKey);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    
                    await request.SendWebRequest().WithCancellation(cancellationToken);

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var res = JsonConvert.DeserializeObject<ModelListResponse>(request.downloadHandler.text);
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
        
    }
}

