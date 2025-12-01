using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Speech;
using Graffity.Groq.Sample.Common; // IViewProperty がここにある想定
using Debug = UnityEngine.Debug;

namespace Graffity.Groq.SpeechSample
{
    public class Property : IResultViewProperty, IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly ReactiveProperty<string> _resultText = new ReactiveProperty<string>(string.Empty);
        private readonly Subject<Unit> _onApiCallStart = new Subject<Unit>();
        private readonly Subject<Unit> _onApiCallCancel = new Subject<Unit>();

        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;
        Subject<Unit> IResultViewProperty.OnApiCallStart => _onApiCallStart;
        Subject<Unit> IResultViewProperty.OnApiCallCancel => _onApiCallCancel;

        public Property()
        {
            _resultText.AddTo(_disposable);
            _onApiCallStart.AddTo(_disposable);
            _onApiCallCancel.AddTo(_disposable);
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

        public ViewModel(IAsyncSpeechRequest<(long statuscode, SpeechResponse res)> apiRequest, IResultViewProperty property)
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
                }).AddTo(_disposable);

            property.OnApiCallStart
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    var result = await apiRequest.SendAsync(cts.Token);
                    property.ResultText.Value = result.res.Text;
                    Debug.Log("Api call done");

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential).AddTo(_disposable);
        }
    }
}
