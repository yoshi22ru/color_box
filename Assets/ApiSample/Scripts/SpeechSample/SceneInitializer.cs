using Cysharp.Threading.Tasks;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Sample.Common;
using Graffity.Groq.Speech;
using UnityEngine;

namespace Graffity.Groq.SpeechSample
{

    public class SceneInitializer : MonoBehaviour
    {
        [SerializeField] private string _apiKey;
        [SerializeField] private string _audioAssetPath;
        [SerializeField] private View _view;

        void Start()
        {
            var p = new Property().AddTo(this);
            
            (_view as IView<IResultViewProperty>).Bind(p);
            var api = new Transcription(_apiKey);
            IAsyncSpeechRequest<(long, SpeechResponse)> speechApi = api;
            speechApi.SetFilePath(_audioAssetPath);

            var model = new ViewModel(api, p).AddTo(this);
            
            
        }
    }
}