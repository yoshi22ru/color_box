using System.Threading;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Sample.Common;
using UnityEngine;

namespace Graffity.Groq.TTSSample
{

    public class SceneInitializer : MonoBehaviour
    {
        [SerializeField] private string _apiKey;
        [SerializeField] private View _view;

        [SerializeField] private TTSEnglishVoiceType _voiceType;
        TTSEnglishVoiceType _prev = TTSEnglishVoiceType.Undefined; 
        void Start()
        {
            var p = new Property().AddTo(this);
            
            (_view as IView<IResultViewProperty>).Bind(p);
            var api = new Speech.Speech(_apiKey);
            var model = new ViewModel(api, p).AddTo(this);
            
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (_prev != _voiceType)
                    {
                        (api as IAsyncTextToSpeechRequest<(long, AudioClip)>).SetVoice(_voiceType.ToWWWField());
                        _prev = _voiceType;
                    }
                })
                .AddTo(this);

        }
    }
}