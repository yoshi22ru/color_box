using System;
using R3;
using TMPro;
using Graffity.Groq.Sample.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Graffity.Groq.FileApiSample
{
    public class View : MonoBehaviour, IView<IResultViewProperty> 
    {
        [SerializeField] private TMP_InputField _inputField = null;
        [SerializeField] private TextMeshProUGUI _resultText = null;
        
        [Header("Buttons")]
        [SerializeField] private Button _uploadButton = null;
        [SerializeField] private Button _deleteButton = null;
        [SerializeField] private Button _getListButton = null;
        [SerializeField] private Button _retrieveButton = null;
        [SerializeField] private Button _downloadButton = null;
        [SerializeField] private Button _cancelButton = null;

        private void Awake()
        {
            Assert.IsNotNull(_inputField, "_inputField is null.");
            Assert.IsNotNull(_resultText, "ResultText is null.");

            Assert.IsNotNull(_uploadButton, "_uploadButton is null.");
            Assert.IsNotNull(_deleteButton, "_deleteButton is null.");
            Assert.IsNotNull(_getListButton, "_getListButton is null.");
            Assert.IsNotNull(_retrieveButton, "_retrieveButton is null.");
            Assert.IsNotNull(_downloadButton, "_downloadButton is null.");
            Assert.IsNotNull(_cancelButton, "_cancelButton is null.");
        }

        void IView<IResultViewProperty>.Bind(IResultViewProperty property)
        {
            property.ResultText.Subscribe(text =>
                {
                    _resultText.text = text;
                })
                .AddTo(this);

            _inputField.onValueChanged.AsObservable()
                .Subscribe(txt =>
                { 
                    property.InputText.Value = txt;
                })
                .AddTo(this);

            _uploadButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnUploadApiCall.OnNext(Unit.Default);
            }).AddTo(this);
            
            _deleteButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnDeleteApiCall.OnNext(Unit.Default);
            }).AddTo(this);
            
            _getListButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnListApiCall.OnNext(Unit.Default);
            }).AddTo(this);
            
            _retrieveButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnRetrieveApiCall.OnNext(Unit.Default);
            }).AddTo(this);
            
            _downloadButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnDownloadApiCall.OnNext(Unit.Default);
            }).AddTo(this);
            
            _cancelButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnApiCallCancel.OnNext(Unit.Default);
            }).AddTo(this);

        }
    }
}