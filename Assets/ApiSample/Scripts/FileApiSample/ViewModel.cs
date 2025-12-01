using System;
using System.Threading;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.File;
using Graffity.Groq.Models;
using Graffity.Groq.Sample.Common;
using UnityEngine;
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

    public class Property : IResultViewProperty, IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly ReactiveProperty<string> _inputText = new ReactiveProperty<string>(string.Empty);
        private readonly ReactiveProperty<string> _resultText = new ReactiveProperty<string>(string.Empty);

        private readonly Subject<Unit> _onUploadApiCall = new Subject<Unit>();
        private readonly Subject<Unit> _onListApiCall = new Subject<Unit>();
        private readonly Subject<Unit> _onDeleteApiCall = new Subject<Unit>();
        private readonly Subject<Unit> _onRetrieveApiCall = new Subject<Unit>();
        private readonly Subject<Unit> _onDownloadApiCall = new Subject<Unit>();
        private readonly Subject<Unit> _onApiCallCanceled = new Subject<Unit>();

        ReactiveProperty<string> IResultViewProperty.InputText => _inputText;
        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;

        Subject<Unit> IResultViewProperty.OnUploadApiCall => _onUploadApiCall;
        Subject<Unit> IResultViewProperty.OnListApiCall => _onListApiCall;
        Subject<Unit> IResultViewProperty.OnDeleteApiCall => _onDeleteApiCall;
        Subject<Unit> IResultViewProperty.OnRetrieveApiCall => _onRetrieveApiCall;
        Subject<Unit> IResultViewProperty.OnDownloadApiCall => _onDownloadApiCall;
        Subject<Unit> IResultViewProperty.OnApiCallCancel => _onApiCallCanceled;

        public Property()
        {
            _inputText.AddTo(_disposable);
            _resultText.AddTo(_disposable);

            _onUploadApiCall.AddTo(_disposable);
            _onListApiCall.AddTo(_disposable);
            _onDeleteApiCall.AddTo(_disposable);
            _onRetrieveApiCall.AddTo(_disposable);
            _onDownloadApiCall.AddTo(_disposable);
            _onApiCallCanceled.AddTo(_disposable);
        }

        public void Dispose()
        {
            Debug.Log("Property disposed");
            _disposable.Dispose();
        }
    }

    public class ViewModel : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public void Dispose()
        {
            Debug.LogWarning("ViewModel disposed");
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
            _disposable.Add(cts);

            property.InputText
                .Subscribe(txt =>
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
                    cts.Cancel();
                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                })
                .AddTo(_disposable);

            property.OnUploadApiCall
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    var result = await uploadApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Upload done: " + result.statusCode);

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential)
                .AddTo(_disposable);

            property.OnListApiCall
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    var result = await getFileListApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("List done: " + result.statusCode);

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential)
                .AddTo(_disposable);

            property.OnDeleteApiCall
                .Where(_ => !string.IsNullOrEmpty(property.InputText.Value))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    var result = await deleteFileApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Delete done: " + result.statusCode);

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential)
                .AddTo(_disposable);

            property.OnRetrieveApiCall
                .Where(_ => !string.IsNullOrEmpty(property.InputText.Value))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    var result = await retrieveFileApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Retrieve done: " + result.statusCode);

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential)
                .AddTo(_disposable);

            property.OnDownloadApiCall
                .Where(_ => !string.IsNullOrEmpty(property.InputText.Value))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    var result = await downloadFileApi.SendAsync(cts.Token);
                    property.ResultText.Value = result.response.ToString();
                    Debug.Log("Download done: " + result.statusCode);

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
        }
    }
}
