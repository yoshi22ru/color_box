using System;
using System.Threading;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.File;
using Graffity.Groq.Models;
using Graffity.Groq.Sample.Common;
using Debug = UnityEngine.Debug;


namespace Graffity.Groq.FileApiSample
{
    public interface IResultViewProperty : IViewProperty
    {
        ReactiveProperty<string> InputText { get; }
        ReactiveProperty<string> ResultText { get; }

        Subject<Unit> OnUploadApiCall { get; }
        Subject<Unit> OnListApiCall { get; }
        Subject<Unit> OnDeleteApiCall { get; }
        Subject<Unit> OnRetrieveApiCall { get; }
        Subject<Unit> OnDownloadApiCall { get; }
        Subject<Unit> OnApiCallCancel { get; }
    }

    
    public class Property : IResultViewProperty 
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        public void Dispose()
        {
            Debug.Log("Property was disposed");
            _disposable.Dispose();
        }

        public Property()
        {
            IResultViewProperty p = this;
            _resultText = new ReactiveProperty<string>(string.Empty).AddTo(_disposable);
            _inputText = new ReactiveProperty<string>(string.Empty).AddTo(_disposable);
            _onUploadApiCall.AddTo(_disposable);
            _onListApiCall.AddTo(_disposable);
            _onDeleteApiCall.AddTo(_disposable);
            _onRetrieveApiCall.AddTo(_disposable);
            _onDownloadApiCall.AddTo(_disposable);
            _onApiCallCanceled.AddTo(_disposable);
        }
        private readonly ReactiveProperty<string> _inputText;
        ReactiveProperty<string> IResultViewProperty.InputText => _inputText;

        
        private readonly ReactiveProperty<string> _resultText;
        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;
        
        Subject<Unit> _onUploadApiCall = new();
        Subject<Unit> IResultViewProperty.OnUploadApiCall => _onUploadApiCall;
        Subject<Unit> _onListApiCall = new();
        Subject<Unit> IResultViewProperty.OnListApiCall => _onListApiCall; 
        Subject<Unit> _onDeleteApiCall = new();
        Subject<Unit> IResultViewProperty.OnDeleteApiCall => _onDeleteApiCall; 
        Subject<Unit> _onRetrieveApiCall = new();
        Subject<Unit> IResultViewProperty.OnRetrieveApiCall => _onRetrieveApiCall; 
        Subject<Unit> _onDownloadApiCall = new();
        Subject<Unit> IResultViewProperty.OnDownloadApiCall => _onDownloadApiCall; 
        
        Subject<Unit> _onApiCallCanceled = new();
        Subject<Unit> IResultViewProperty.OnApiCallCancel => _onApiCallCanceled;

    }


    public class ViewModel : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        public void Dispose()
        {
            Debug.LogWarning("ViewModel is disposed");
            _disposable.Dispose();
        }


        public ViewModel(
            FileUploadApi uploadApi,
            GetFileListApi getFileListApi,
            DeleteFileApi deleteFileApi,
            RetrieveFileApi retrieveFileApi,
            DownloadFileApi downloadFileApi,
            IResultViewProperty property)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            
            property.InputText.Subscribe(txt =>
                {
                    uploadApi.SetFilePath(txt);
                    deleteFileApi.SetFileId(txt);
                    retrieveFileApi.SetFileId(txt);
                    downloadFileApi.SetFileId(txt);
                })
                .AddTo(_disposable);
            property.OnApiCallCancel
                .Subscribe(_ =>
                {
                    Debug.Log("Api call cancelled");
                    cts?.Cancel();
                    cts = new CancellationTokenSource().AddTo(_disposable);
                }).AddTo(_disposable);
            
            property.OnUploadApiCall
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    var result = await uploadApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Api call done: "+result.statusCode);
                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
            property.OnListApiCall
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    var result = await getFileListApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Api call done: "+result.statusCode);
                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
            
            property.OnDeleteApiCall
                .Where(_ => !string.IsNullOrEmpty(property.InputText.Value))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    var result = await deleteFileApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Api call done: "+result.statusCode);
                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
            
            property.OnRetrieveApiCall
                .Where(_ => !string.IsNullOrEmpty(property.InputText.Value))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    var result = await retrieveFileApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Api call done: "+result.statusCode);
                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
            
            property.OnDownloadApiCall
                .Where(_ => !string.IsNullOrEmpty(property.InputText.Value))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    var result = await downloadFileApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Api call done: "+result.statusCode);
                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
        }
    }
}