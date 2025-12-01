using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Graffity.Groq.LLMSample
{
    public class Property : IResultViewProperty, IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly ReactiveProperty<string> _prompt = new ReactiveProperty<string>(string.Empty);
        private readonly ReactiveProperty<string> _resultText = new ReactiveProperty<string>(string.Empty);

        private readonly Subject<Unit> _onApiCallStart = new Subject<Unit>();
        private readonly Subject<Unit> _onApiCallCanceled = new Subject<Unit>();

        ReactiveProperty<string> IResultViewProperty.Prompt => _prompt;
        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;

        Subject<Unit> IResultViewProperty.OnApiCallStart => _onApiCallStart;
        Subject<Unit> IResultViewProperty.OnApiCallCancel => _onApiCallCanceled;

        public Property()
        {
            _prompt.AddTo(_disposable);
            _resultText.AddTo(_disposable);

            _onApiCallStart.AddTo(_disposable);
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
            Debug.LogWarning("ViewModel is disposed");
            _disposable.Dispose();
        }

        public ViewModel(IAsyncChatRequest<(long statuscode, ChatCompletionResponse res)> apiRequest, IResultViewProperty property)
        {
            Debug.Log("Init ViewModel");
            CancellationTokenSource cts = new CancellationTokenSource();
            _disposable.Add(cts);

            property.OnApiCallCancel
                .Subscribe(_ =>
                {
                    Debug.Log("Api call cancelled");
                    cts.Cancel();
                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                })
                .AddTo(_disposable);

            property.OnApiCallStart
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    apiRequest.SetPrompt(new UserMessage(property.Prompt.Value));

                    var result = await apiRequest.SendAsync(cts.Token);
                    var reply = result.res?.Choices?.FirstOrDefault()?.Message?.Content;

                    Debug.Log(reply);
                    property.ResultText.Value = reply;
                    Debug.Log("Api call done");

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
        }
    }
}
