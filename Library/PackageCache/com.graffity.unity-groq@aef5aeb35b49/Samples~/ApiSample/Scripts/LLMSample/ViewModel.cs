using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Text;
using Debug = UnityEngine.Debug;


namespace Graffity.Groq.LLMSample
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
            _prompt = new ReactiveProperty<string>(string.Empty).AddTo(_disposable);
            _resultText = new ReactiveProperty<string>(string.Empty).AddTo(_disposable);
            _onApiCallStart.AddTo(_disposable);
            _onApiCallCanceled.AddTo(_disposable);
        }
        private ReactiveProperty<string> _prompt;
        ReactiveProperty<string> IResultViewProperty.Prompt => _prompt;
        
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


        public ViewModel(IAsyncChatRequest<(long statuscode, ChatCompletionResponse res)> apiRequest, IResultViewProperty property)
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
                    apiRequest.SetPrompt(new UserMessage(property.Prompt.CurrentValue));
                    IAsyncRequest<(long statuscode, ChatCompletionResponse res)> api = apiRequest;
                    var result = await api.SendAsync(cts.Token);
                    var reply = result.res?.Choices?.FirstOrDefault()?.Message?.Content;
                    Debug.Log(reply);
                    property.ResultText.Value =reply;
                    Debug.Log("Api call done");

                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
        }
    }
}