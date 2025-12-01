using System;
using System.Diagnostics;
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
            _clip = new ReactiveProperty<AudioClip>(null).AddTo(_disposable);
            _onApiCallStart.AddTo(_disposable);
            _onApiCallCanceled.AddTo(_disposable);
            _onAudioPlay.AddTo(_disposable);
        }
        private readonly ReactiveProperty<string> _inputText;
        ReactiveProperty<string> IResultViewProperty.InputText => _inputText;

        
        private readonly ReactiveProperty<string> _resultText;
        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;

        private readonly ReactiveProperty<AudioClip> _clip;
        ReactiveProperty<AudioClip> IResultViewProperty.Clip => _clip;

        
        Subject<Unit> _onApiCallStart = new();
        Subject<Unit> IResultViewProperty.OnApiCallStart => _onApiCallStart;
        Subject<Unit> _onApiCallCanceled = new();
        Subject<Unit> IResultViewProperty.OnApiCallCancel => _onApiCallCanceled;

        Subject<Unit> _onAudioPlay = new();
        Subject<Unit> IResultViewProperty.OnAudioPlay => _onAudioPlay;

    }


    public class ViewModel : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        public void Dispose()
        {
            Debug.LogWarning("ViewModel is disposed");
            _disposable.Dispose();
        }


        public ViewModel(IAsyncTextToSpeechRequest<(long statuscode, AudioClip res)> apiRequest, IResultViewProperty property)
        {
            Debug.Log("Init ViewModel");
            apiRequest.SetModel(TTSAiModelType.playai_tts);

            CancellationTokenSource cts = new CancellationTokenSource();
            
            property.InputText.Subscribe(txt =>
                {
                    apiRequest.SetText(txt);
                })
                .AddTo(_disposable);
            property.OnApiCallCancel
                .Subscribe(_ =>
                {
                    Debug.Log("Api call cancelled");
                    cts?.Cancel();
                    cts = new CancellationTokenSource().AddTo(_disposable);
                }).AddTo(_disposable);
            
            
            property.OnApiCallStart
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Api call start");
                    Debug.Log("Text:"+apiRequest.Text);
                    IAsyncRequest<(long statuscode, AudioClip clip)> api = apiRequest;
                    var result = await api.SendAsync(cts.Token);
                    property.ResultText.Value = result.statuscode.ToString();
                    property.Clip.Value = result.clip;
                    Debug.Log("Api call done: "+result.statuscode);

                    cts = new CancellationTokenSource().AddTo(_disposable);

                }, AwaitOperation.Sequential)
                .AddTo(_disposable);
        }
    }
}