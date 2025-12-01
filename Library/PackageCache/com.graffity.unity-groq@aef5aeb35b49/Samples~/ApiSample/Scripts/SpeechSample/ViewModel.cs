using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Speech;
using Debug = UnityEngine.Debug;


namespace Graffity.Groq.SpeechSample
{
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
            _onApiCallStart.AddTo(_disposable);
            _onApiCallCanceled.AddTo(_disposable);
        }
        
        private ReactiveProperty<string> _resultText;
        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;
        
        Subject<Unit> _onApiCallStart = new();
        Subject<Unit> IResultViewProperty.OnApiCallStart => _onApiCallStart;
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


        public ViewModel(IAsyncSpeechRequest<(long statuscode, SpeechResponse res)> apiRequest, IResultViewProperty property)
        {
            Debug.Log("Init ViewModel");
            CancellationTokenSource cts = new CancellationTokenSource();
            property.OnApiCallCancel
                .Subscribe(_ =>
                {
                    Debug.Log("Api call cancelled");
                    cts?.Cancel();
                    cts = new CancellationTokenSource().AddTo(_disposable);
                }).AddTo(_disposable);
            
            
            property.OnApiCallStart
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    IAsyncRequest<(long statuscode, SpeechResponse res)> api = apiRequest;
                    var result = await api.SendAsync(cts.Token);
                    Debug.Log(result.res.Text);
                    property.ResultText.Value = result.res.Text;
                    Debug.Log("Api call done");

                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
        }
    }
}