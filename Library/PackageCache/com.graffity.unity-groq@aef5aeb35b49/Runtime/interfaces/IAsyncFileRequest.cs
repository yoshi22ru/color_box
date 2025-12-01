namespace Graffity.Groq.Common
{
    public interface IAsyncFileRequest<TResponse> : IAsyncRequest<TResponse>
    {
        string FileId { get; }
        IAsyncFileRequest<TResponse> SetFileId(string fileId);
    }
    
    public interface IAsyncFileUploadRequest<TResponse> : IAsyncRequest<TResponse>
    {
        string FilePath { get; }
        IAsyncFileUploadRequest<TResponse> SetFilePath(string path);
    }
}