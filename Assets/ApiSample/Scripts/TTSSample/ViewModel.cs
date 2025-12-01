using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Sample.Common;
using Graffity.Groq.Speech;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Graffity.Groq.TTSSample
{
    public interface IResultViewProperty : IViewProperty
    {
        ReactiveProperty<string> InputText { get; }
        ReactiveProperty<string> ResultText { get; }
        ReactiveProperty<AudioClip> Clip { get; }

        Subject<Unit> OnApiCallStart { get; }
        Subject<Unit> OnApiCallCancel { get; }
        Subject<Unit> OnAudioPlay { get; }
    }

    public class Property : IResultViewProperty, IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly ReactiveProperty<string> _inputText = new ReactiveProperty<string>(string.Empty);
        private readonly ReactiveProperty<string> _resultText = new ReactiveProperty<string>(string.Empty);
        private readonly ReactiveProperty<AudioClip> _clip = new ReactiveProperty<AudioClip>(null);

        private readonly Subject<Unit> _onApiCallStart = new Subject<Unit>();
        private readonly Subject<Unit> _onApiCallCanceled = new Subject<Unit>();
        private readonly Subject<Unit> _onAudioPlay = new Subject<Unit>();

        ReactiveProperty<string> IResultViewProperty.InputText => _inputText;
        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;
        ReactiveProperty<AudioClip> IResultViewProperty.Clip => _clip;

        Subject<Unit> IResultViewProperty.OnApiCallStart => _onApiCallStart;
        Subject<Unit> IResultViewProperty.OnApiCallCancel => _onApiCallCanceled;
        Subject<Unit> IResultViewProperty.OnAudioPlay => _onAudioPlay;

        public Property()
        {
            // AddTo は代入せず呼び出すだけ
            _inputText.AddTo(_disposable);
            _resultText.AddTo(_disposable);
            _clip.AddTo(_disposable);

            _onApiCallStart.AddTo(_disposable);
            _onApiCallCanceled.AddTo(_disposable);
            _onAudioPlay.AddTo(_disposable);
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

        public ViewModel(IAsyncTextToSpeechRequest<(long statuscode, AudioClip res)> apiRequest, IResultViewProperty property)
        {
            Debug.Log("Init ViewModel");
            apiRequest.SetModel(TTSAiModelType.playai_tts);

            // CancellationTokenSource は AddTo では代入しない
            CancellationTokenSource cts = new CancellationTokenSource();
            _disposable.Add(cts);

            property.InputText
                .Subscribe(txt => apiRequest.SetText(txt))
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

            property.OnApiCallStart
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    Debug.Log("Text:" + apiRequest.Text);

                    var result = await apiRequest.SendAsync(cts.Token);
                    property.ResultText.Value = result.statuscode.ToString();
                    property.Clip.Value = result.res;

                    Debug.Log("Api call done: " + result.statuscode);

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
        }
    }
}
