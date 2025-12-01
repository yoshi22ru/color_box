using System;
using System.Threading;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Models;
using Graffity.Groq.Sample.Common;
using Debug = UnityEngine.Debug;


namespace Graffity.Groq.ModelApiSample
{
    public interface IResultViewProperty : IViewProperty
    {
        ReactiveProperty<string> InputText { get; }
        ReactiveProperty<string> ResultText { get; }

        Subject<Unit> OnListApiCallStart { get; }
        Subject<Unit> OnDetailApiCallStart { get; }
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
            _onlistApiCall.AddTo(_disposable);
            _onDetailApiCall.AddTo(_disposable);
            _onApiCallCanceled.AddTo(_disposable);
        }
        private readonly ReactiveProperty<string> _inputText;
        ReactiveProperty<string> IResultViewProperty.InputText => _inputText;

        
        private readonly ReactiveProperty<string> _resultText;
        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;
        
        
        Subject<Unit> _onlistApiCall = new();
        Subject<Unit> IResultViewProperty.OnListApiCallStart => _onlistApiCall;
        Subject<Unit> _onDetailApiCall = new();
        Subject<Unit> IResultViewProperty.OnDetailApiCallStart => _onDetailApiCall;
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


        public ViewModel(IAsyncRequest<(long statuscode, ModelListResponse res)> listApiRequest, 
            IAsyncModelRequest<(long statuscode, ModelResponse res)> detailApiRequest, 
            IResultViewProperty property)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            
            property.InputText.Subscribe(txt =>
                {
                    detailApiRequest.SetModelName(txt);
                })
                .AddTo(_disposable);
            property.OnApiCallCancel
                .Subscribe(_ =>
                {
                    Debug.Log("Api call cancelled");
                    cts?.Cancel();
                    cts = new CancellationTokenSource().AddTo(_disposable);
                }).AddTo(_disposable);
            
            
            property.OnListApiCallStart
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    var result = await listApiRequest.SendAsync(cts.Token);
                    property.ResultText.Value = result.res.ToString();
                    Debug.Log("Api call done: "+result.statuscode);
                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
            
            property.OnDetailApiCallStart
                .Where(_ => !string.IsNullOrEmpty(property.InputText.Value))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    detailApiRequest.SetModelName(property.InputText.Value);

                    IAsyncRequest<(long statuscode, ModelResponse res)> api = detailApiRequest;
                    var result = await api.SendAsync(cts.Token);
                    property.ResultText.Value = result.res.ToString();
                    Debug.Log("Api call done: "+result.statuscode);

                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
        }
    }
}