using System;
using R3;
using TMPro;
using Graffity.Groq.Sample.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Graffity.Groq.TTSSample
{
    public class View : MonoBehaviour, IView<IResultViewProperty> 
    {
        [SerializeField] private TMP_InputField _inputField = null;
        [SerializeField] private TextMeshProUGUI _resultText = null;
        [SerializeField] private AudioSource _audioSource = null;

        
        [SerializeField] private Button _startButton = null;
        [SerializeField] private Button _cancelButton = null;
        [SerializeField] private Button _playButton = null;

        private void Awake()
        {
            Assert.IsNotNull(_inputField, "_inputField is null.");
            Assert.IsNotNull(_resultText, "ResultText is null.");
            Assert.IsNotNull(_audioSource, "_audioSource is null.");

            Assert.IsNotNull(_startButton, "_startButton is null.");
            Assert.IsNotNull(_cancelButton, "_cancelButton is null.");
            Assert.IsNotNull(_playButton, "_playButton is null.");
        }

        void IView<IResultViewProperty>.Bind(IResultViewProperty property)
        {
            property.ResultText.Subscribe(text =>
                {
                    _resultText.text = "StatusCode:"+text;
                })
                .AddTo(this);

            _inputField.onValueChanged.AsObservable()
                .Subscribe(txt =>
                { 
                    property.InputText.Value = txt;
                })
                .AddTo(this);
            property.OnAudioPlay
                .Subscribe(_ =>
                {
                    if (_audioSource?.clip != null)
                    {
                        _audioSource.Stop();
                        _audioSource.Play();
                    }
                })
                .AddTo(this);
            
            property.Clip.Subscribe(clip =>
                {
                    _audioSource.clip = clip;
                    if (clip != null && clip.samples != 0)
                    {
                        _audioSource.Play();
                    }
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

            _playButton.OnClickAsObservable().Subscribe(_ =>
            {
                property.OnAudioPlay.OnNext(Unit.Default);
            }).AddTo(this);
            
        }
    }
}