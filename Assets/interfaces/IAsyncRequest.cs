using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Graffity.Groq.Common;

namespace Graffity.Groq.Common
{
    public interface IAsyncRequest<TResponse>
    {
        string Endpoint { get; }
        string ApiKey { get; }

        /// <summary>
        /// API Call
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>response</returns>
        UniTask<TResponse> SendAsync(CancellationToken cancellationToken);
    }
}