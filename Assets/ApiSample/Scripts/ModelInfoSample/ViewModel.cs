using System;
using System.Threading;
using R3;
using Graffity.Groq.Common;
using Graffity.Groq.Models;
using Graffity.Groq.Sample.Common;
using Debug = UnityEngine.Debug;

namespace Graffity.Groq.ModelApiSample
{
    public interface IResultViewProperty : IViewProperty
    {
        ReactiveProperty<string> InputText { get; }
        ReactiveProperty<string> ResultText { get; }

        Subject<Unit> OnListApiCallStart { get; }
        Subject<Unit> OnDetailApiCallStart { get; }
        Subject<Unit> OnApiCallCancel { get; }
    }

    public class Property : IResultViewProperty
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly ReactiveProperty<string> _inputText;
        ReactiveProperty<string> IResultViewProperty.InputText => _inputText;

        private readonly ReactiveProperty<string> _resultText;
        ReactiveProperty<string> IResultViewProperty.ResultText => _resultText;

        private readonly Subject<Unit> _onlistApiCall;
        Subject<Unit> IResultViewProperty.OnListApiCallStart => _onlistApiCall;

        private readonly Subject<Unit> _onDetailApiCall;
        Subject<Unit> IResultViewProperty.OnDetailApiCallStart => _onDetailApiCall;

        private readonly Subject<Unit> _onApiCallCanceled;
        Subject<Unit> IResultViewProperty.OnApiCallCancel => _onApiCallCanceled;

        public Property()
        {
            _inputText = new ReactiveProperty<string>(string.Empty);
            _inputText.AddTo(_disposable);

            _resultText = new ReactiveProperty<string>(string.Empty);
            _resultText.AddTo(_disposable);

            _onlistApiCall = new Subject<Unit>();
            _onlistApiCall.AddTo(_disposable);

            _onDetailApiCall = new Subject<Unit>();
            _onDetailApiCall.AddTo(_disposable);

            _onApiCallCanceled = new Subject<Unit>();
            _onApiCallCanceled.AddTo(_disposable);
        }

        public void Dispose()
        {
            Debug.Log("Property disposed");
            _disposable.Dispose();
        }
    }

    public class ViewModel : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public void Dispose()
        {
            Debug.LogWarning("ViewModel disposed");
            _disposable.Dispose();
        }

        public ViewModel(
            IAsyncRequest<(long statuscode, ModelListResponse res)> listApiRequest,
            IAsyncModelRequest<(long statuscode, ModelResponse res)> detailApiRequest,
            IResultViewProperty property)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            _disposable.Add(cts);

            property.InputText.Subscribe(txt =>
            {
                detailApiRequest.SetModelName(txt);
            }).AddTo(_disposable);

            property.OnApiCallCancel.Subscribe(_ =>
            {
                Debug.Log("Api call cancelled");
                cts.Cancel();
                cts = new CancellationTokenSource();
                _disposable.Add(cts);
            }).AddTo(_disposable);

            property.OnListApiCallStart
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("List API call start");
                    var result = await listApiRequest.SendAsync(cts.Token);
                    property.ResultText.Value = result.res.ToString();
                    Debug.Log("List API call done: " + result.statuscode);

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential).AddTo(_disposable);

            property.OnDetailApiCallStart
                .Where(_ => !string.IsNullOrEmpty(property.InputText.Value))
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .SubscribeAwait(async (_, token) =>
                {
                    Debug.Log("Detail API call start");
                    detailApiRequest.SetModelName(property.InputText.Value);

                    var result = await detailApiRequest.SendAsync(cts.Token);
                    property.ResultText.Value = result.res.ToString();
                    Debug.Log("Detail API call done: " + result.statuscode);

                    cts = new CancellationTokenSource();
                    _disposable.Add(cts);
                }, AwaitOperation.Sequential).AddTo(_disposable);
        }
    }
}
