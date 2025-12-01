namespace Graffity.Groq.Common
{

    
    public interface IAsyncModelRequest<TResponse> : IAsyncRequest<TResponse>
    {
        /// <summary>
        /// for REST Endpoint 
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns>this</returns>
        IAsyncModelRequest<TResponse> SetModelName(string modelName);
        string ModelName { get; }
    }
}