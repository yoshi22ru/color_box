using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Text;
using Graffity.Groq.Sample.Common;

using UnityEngine;

namespace Graffity.Groq.LLMSample
{
    public class SceneInitializer : MonoBehaviour
    {
        [SerializeField] private string _apiKey;
        
        [SerializeField, TextArea] private string _prompt;
        [SerializeField] private View _view;

        void Start()
        {
            var p = new Property().AddTo(this);
            (p as IResultViewProperty).Prompt.Value= _prompt;
            
            (_view as IView<IResultViewProperty>).Bind(p);
            var api = new ChatCompletion(_apiKey);
            api.SetPrompt(new UserMessage(_prompt) );
            var model = new ViewModel(api, p).AddTo(this);
            
            
        }
    }
}
