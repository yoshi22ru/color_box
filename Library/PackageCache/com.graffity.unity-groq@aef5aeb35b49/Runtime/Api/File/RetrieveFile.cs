using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Graffity.Groq.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Graffity.Groq.File
{
    /// <summary>
    /// Get File
    /// </summary>
    /// <see cref="https://console.groq.com/docs/api-reference#files-retrieve"/>
    public class RetrieveFileApi : IAsyncFileRequest<(long statusCode, FileResponse response)>
    {
        private readonly string _endpoint = "https://api.groq.com/openai/v1/files/{0}";
        private readonly string _apiKey = string.Empty;
        public RetrieveFileApi(string ApiKey)
        {
            _apiKey = ApiKey;
        }

        #region ===== parameters =====

        [Header("Required")]
        private string _fileId = string.Empty;

        #endregion //) ===== parameters =====
        
        #region IAsyncRequest implementation

        public string Endpoint => string.Intern(string.Format(_endpoint, FileId));
        public string ApiKey => _apiKey;

        /// <summary>
        /// API call
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>StatusCode, FileResponse</returns>
        public async UniTask<(long statusCode, FileResponse response)> SendAsync(CancellationToken cancellationToken)
        {
            try
            {
                // UnityWebRequest を作成
                using (UnityWebRequest request = new UnityWebRequest(Endpoint, "Get"))
                {
                    // Authorization ヘッダーを追加
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", "Bearer " + ApiKey);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    
                    await request.SendWebRequest().WithCancellation(cancellationToken);

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var res = JsonConvert.DeserializeObject<FileResponse>(request.downloadHandler.text);
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
        
        #region ===== IAsyncFileRequest =====

        public string FileId => _fileId;

        public IAsyncFileRequest<(long statusCode, FileResponse response)> SetFileId(string fileId)
        {
            this._fileId = fileId;
            return this;
        }

        #endregion //) ===== IAsyncFileRequest =====
    }
}

