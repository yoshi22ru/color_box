using System;
using R3;
using TMPro;
using Graffity.Groq.Sample.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Graffity.Groq.ModelApiSample
{
    public class View : MonoBehaviour, IView<IResultViewProperty> 
    {
        [SerializeField] private TMP_InputField _inputField = null;
        [SerializeField] private TextMeshProUGUI _resultText = null;

        
        [SerializeField] private Button _startButton = null;
        [SerializeField] private Button _detailButton = null;
        [SerializeField] private Button _cancelButton = null;

        private void Awake()
        {
            Assert.IsNotNull(_inputField, "_inputField is null.");
            Assert.IsNotNull(_resultText, "ResultText is null.");

            Assert.IsNotNull(_startButton, "_startButton is null.");
            Assert.IsNotNull(_cancelButton, "_cancelButton is null.");
            Assert.IsNotNull(_detailButton, "_detailButton is null.");
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

            _startButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnListApiCallStart.OnNext(Unit.Default);
            }).AddTo(this);
            
            _cancelButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnApiCallCancel.OnNext(Unit.Default);
            }).AddTo(this);

            _detailButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnDetailApiCallStart.OnNext(Unit.Default);
            }).AddTo(this);
            
        }
    }
}