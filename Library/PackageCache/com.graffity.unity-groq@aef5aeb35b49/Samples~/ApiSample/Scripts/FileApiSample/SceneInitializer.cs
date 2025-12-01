using R3;
using Graffity.Groq.File;
using Graffity.Groq.Models;
using Graffity.Groq.Sample.Common;
using UnityEngine;

namespace Graffity.Groq.FileApiSample
{

    public class SceneInitializer : MonoBehaviour
    {
        [SerializeField] private string _apiKey;
        [SerializeField] private View _view;

        void Start()
        {
            var p = new Property().AddTo(this);
            
            (_view as IView<IResultViewProperty>).Bind(p);
            var uploadApi = new FileUploadApi(_apiKey);
            var listApi = new GetFileListApi(_apiKey);
            var deleteApi = new DeleteFileApi(_apiKey);
            var retrieveApi = new RetrieveFileApi(_apiKey);
            var downloadApi = new DownloadFileApi(_apiKey);

            new ViewModel(uploadApi,
                listApi,
                deleteApi,
                retrieveApi,
                downloadApi,
                p).AddTo(this);
        }
    }
}