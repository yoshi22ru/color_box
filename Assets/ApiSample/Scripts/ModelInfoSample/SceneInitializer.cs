using System.Threading;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Models;
using Graffity.Groq.Sample.Common;
using UnityEngine;

namespace Graffity.Groq.ModelApiSample
{

    public class SceneInitializer : MonoBehaviour
    {
        [SerializeField] private string _apiKey;
        [SerializeField] private View _view;

        void Start()
        {
            var p = new Property().AddTo(this);
            
            (_view as IView<IResultViewProperty>).Bind(p);
            var listApi = new ModelListApi(_apiKey);
            var detailApi = new ModelDetailApi(_apiKey);
            new ViewModel(listApi,detailApi, p).AddTo(this);
        }
    }
}