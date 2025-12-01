using System;
using R3;
using TMPro;
using Graffity.Groq.Sample.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Graffity.Groq.SpeechSample
{
    public interface IResultViewProperty : IViewProperty
    {
        ReactiveProperty<string> ResultText { get; }
        Subject<Unit> OnApiCallStart { get; }
        Subject<Unit> OnApiCallCancel { get; }
    }
    
    public class View : MonoBehaviour, IView<IResultViewProperty> 
    {
        [SerializeField] private TextMeshProUGUI _resultText = null;

        
        [SerializeField] private Button _startButton = null;
        [SerializeField] private Button _cancelButton = null;

        private void Awake()
        {
            Assert.IsNotNull(_resultText, "ResultText is null.");
        }

        void IView<IResultViewProperty>.Bind(IResultViewProperty property)
        {
            property.ResultText.Subscribe(text =>
                {
                    Debug.Log(text);
                    _resultText.text = text;
                })
                .AddTo(this);


            _startButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnApiCallStart.OnNext(Unit.Default);
            }).AddTo(this);
            
            _cancelButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnApiCallCancel.OnNext(Unit.Default);
            }).AddTo(this);

        }
    }
}