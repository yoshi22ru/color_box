using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Graffity.Groq.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Graffity.Groq.File
{
    /// <summary>
    /// File Upload POST Api
    /// </summary>
    /// <see cref="https://console.groq.com/docs/api-reference#files-upload"/>
    public class FileUploadApi : IAsyncFileUploadRequest<(long statusCode, FileResponse response)>
    {

        private readonly string _endpoint = "https://api.groq.com/openai/v1/files";
        private readonly string _apiKey = string.Empty;
        public FileUploadApi(string ApiKey)
        {
            _apiKey = ApiKey;
        }
        #region ===== parameters =====

        [Header("Required")]
        private string _filePath = string.Empty;
        private string _purpose = "batch";

        #endregion //) ===== parameters =====

        #region IAsyncRequest implementation
        public string Endpoint => _endpoint;
        public string ApiKey => _apiKey;

        /// <summary>
        /// API call
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>StatusCode, SpeechResponse</returns>
        public async UniTask<(long statusCode, FileResponse response)> SendAsync(CancellationToken cancellationToken)
        {
            try
            {
                var jsonData = CreateJsonRequest();
                
                // Create UnityWebRequest
                using (UnityWebRequest request = new UnityWebRequest(Endpoint, "Post"))
                {
                    // Add Headers
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", "Bearer " + ApiKey);

                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();

                    
                    await request.SendWebRequest().WithCancellation(cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Failed to Create Audio file");
                        return (request.responseCode, null);
                    }

                    return (request.responseCode, JsonConvert.DeserializeObject<FileResponse>(request.downloadHandler.text));
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

        private string CreateJsonRequest()
        {
            var requestBody = new RequestBody();
            // Required
            requestBody.file = FilePath;
            requestBody.purpose = _purpose;
            
            return JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            });
        }


        [System.Serializable]
        public class RequestBody
        {
            public string purpose = "batch";
            public string file;
        }

        #endregion //) IAsyncRequest implementation

        #region ===== IAsyncFileUploadRequest =====

        public string FilePath => _filePath;

        public IAsyncFileUploadRequest<(long statusCode, FileResponse response)> SetFilePath(string path)
        {
            this._filePath = path;
            return this;
        }

        #endregion //) ===== IAsyncFileUploadRequest =====

    }
    
}
