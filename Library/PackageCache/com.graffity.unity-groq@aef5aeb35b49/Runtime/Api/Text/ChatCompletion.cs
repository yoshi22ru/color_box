using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Graffity.Groq.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Graffity.Groq.Text
{
    /// <summary>
    /// Chat API
    /// </summary>
    /// <see cref="https://console.groq.com/docs/api-reference#chat-create"/>
    public class ChatCompletion : IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)>
    {
        private readonly ChatCompletionResponse CanceledResponse = new ChatCompletionResponse()
        {
            Choices = new Choice[]
            {
                new Choice() { Message = new Message() { Role = "error", Content = "" } }
            }
        };

        private readonly string _endpoint = "https://api.groq.com/openai/v1/chat/completions";
        private readonly string _apiKey = string.Empty;
        public ChatCompletion(string ApiKey)
        {
            _apiKey = ApiKey;
        }

        #region ===== parameters =====

        [Header("Required")] private BaseMessageContent _messageContent = null;
        private ChatAiModelType _modelType = ChatAiModelType.LLAMA_3_3_70b_versatile;

        [Header("Optional")] private float _freqPenalty = 0f;
        private Object _logitBias = null;
        private bool? _logProbs = null;
        private int? _maxCompletionTokens = null;
        private int? _n = null;
        private bool? _parallelToolCalls = null;
        private int? _presencePenalty = null;
        private string _reasoningFormat = null;
        private int? _seed = null;
        private string _serviceTier = null;
        private string _stopMessage = null;
        private float _temperture = 0f;
        private string _toolChoice = "none";
        private int? _topLogprob = null;
        private float? _topP = null;
        private string _user = null;

        #endregion //) ===== parameters =====

        #region IAsyncRequest implementation

        public string Endpoint => _endpoint;
        public string ApiKey => _apiKey;

        /// <summary>
        /// API call
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>StatusCode, ChatCompletionResponse</returns>
        public async UniTask<(long statusCode, ChatCompletionResponse response)> SendAsync(CancellationToken cancellationToken)
        {
            IAsyncRequest<(long statusCode, ChatCompletionResponse response)> api = this;
            try
            {
                // フォームデータを作成
                string jsonData = CreatePostData();
                // UnityWebRequest を作成
                using (UnityWebRequest request = new UnityWebRequest(api.Endpoint, "Post"))
                {
                    // Authorization ヘッダーを追加
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", "Bearer " + api.ApiKey);

                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    
                    await request.SendWebRequest().WithCancellation(cancellationToken);

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var res = JsonConvert.DeserializeObject<ChatCompletionResponse>(request.downloadHandler.text);
                        return (request.responseCode, res);
                    }
                    else
                    {
                        var res = new ChatCompletionResponse()
                        {
                            Choices = new Choice[]
                            {
                                new Choice() { Message = new Message() { Role = "error", Content = request.error } }
                            }
                        };
                        return (request.responseCode, res);
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning(e.ToString());
                return (408, CanceledResponse);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
        }

        private string CreatePostData()
        {
            var body = new RequestBody();
            IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> property = this;
            // Required
            body.messages = new [] { new RequestMessage(property.Prompt) };
            body.model = property.ModelType.ToModelID();
            // Optional
            if (property.FreqPenalty != 0) body.frequency_penalty = property.FreqPenalty;
            // if (property.LogitBias != null ) body.logit_bias = property.LogitBias;
            // if (property.LogProbs != null ) body.logprobs = property.LogProbs;
            if (property.MaxCompletionTokens != null) body.max_completion_tokens = property.MaxCompletionTokens;
            if (property.N != null) body.n = property.N;
            if (property.ParallelToolCalls != null) body.parallel_tool_calls = property.ParallelToolCalls;
            if (property.PresencePenalty != null) body.presence_penalty = property.PresencePenalty;
            if (!string.IsNullOrEmpty(property.ReasoningFormat)) body.reasoning_format = property.ReasoningFormat;
            if (property.Seed != null) body.seed = property.Seed;
            if (!string.IsNullOrEmpty(property.ServiceTier)) body.service_tier = property.ServiceTier;
            if (!string.IsNullOrEmpty(property.StopMessage)) body.stop = property.StopMessage;
            if (property.Temperture != 0) body.temperature = property.Temperture;
            if (!string.IsNullOrEmpty(property.ToolChoice)) body.tool_choice = property.ToolChoice;
            if (property.TopLogprob != null) body.top_logprobs = property.TopLogprob;
            if (property.TopP != null) body.top_p = property.TopP;
            if (!string.IsNullOrEmpty(property.User)) body.user = property.User;
            
            return JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            });
        }

        #endregion //) IAsyncRequest implementation

        #region ===== IAsyncChatRequest =====


        /// <summary>
        /// Audio File Path
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="filePath"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetPrompt(BaseMessageContent message)
        {
            _messageContent = message;
            return this;
        }

        public BaseMessageContent Prompt => _messageContent;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="modelType"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetModel(ChatAiModelType modelType)
        {
            _modelType = modelType;
            return this;
        }

        public ChatAiModelType ModelType => _modelType;

        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize
        /// new tokens based on their existing frequency in the text so far,
        /// decreasing the model's likelihood to repeat the same line verbatim.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[-2.0 2.0] default=0</param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetFreqPenalty(float value)
        {
            _freqPenalty = value;
            return this;
        }

        public float FreqPenalty => _freqPenalty;

        /// <summary>
        /// This is not yet supported by any of our models.
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetLogitBias(Object value)
        {
            _logitBias = value;
            return this;
        }

        public Object LogitBias => _logitBias;



        /// <summary>
        /// This is not yet supported by any of our models.
        /// Whether to return log probabilities of the output tokens or not.
        /// If true, returns the log probabilities of each output token returned in the content of message.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetLogProbs(bool? value)
        {
            _logProbs = value;
            return this;
        }

        public bool? LogProbs => _logProbs;

        /// <summary>
        /// The maximum number of tokens that can be generated in the chat completion.
        /// The total length of input tokens and generated tokens is limited by the model's context length.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="tokenCount"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetMaxCompletionTokens(
                int? tokenCount)
        {
            _maxCompletionTokens = tokenCount;
            return this;
        }

        public int? MaxCompletionTokens => _maxCompletionTokens;

        /// <summary>
        /// How many chat completion choices to generate for each input message.
        /// Note that the current moment, only n=1 is supported. Other values will result in a 400 response.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="count"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetChoiceCount(int? count)
        {
            _n = count;
            return this;
        }

        public int? N => _n;

        /// <summary>
        /// Whether to enable parallel function calling during tool use.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetParallelToolCalls(bool? value)
        {
            _parallelToolCalls = value;
            return this;
        }

        public bool? ParallelToolCalls => _parallelToolCalls;

        /// <summary>
        /// Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on whether they appear in the text so far,
        /// increasing the model's likelihood to talk about new topics.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[-2.0 2.0] default=0</param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetPresencePenalty(int? value)
        {
            _presencePenalty = value;
            return this;
        }

        public int? PresencePenalty => _presencePenalty;

        /// <summary>
        /// Specifies how to output reasoning tokens
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetReasoningFormat(string value)
        {
            _reasoningFormat = value;
            return this;
        }

        public  string ReasoningFormat => _reasoningFormat;

        /// <summary>
        /// If specified, our system will make a best effort to sample deterministically,
        /// such that repeated requests with the same seed and parameters should return the same result.
        /// Determinism is not guaranteed, and you should refer to the system_fingerprint response parameter
        /// to monitor changes in the backend.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[-2.0 2.0] default=0</param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetSeed(int? value)
        {
            _seed = value;
            return this;
        }

        public int? Seed => _seed;

        /// <summary>
        /// The service tier to use for the request. Defaults to on_demand.
        /// auto will automatically select the highest tier available within the rate limits of your organization.
        /// flex uses the flex tier, which will succeed or fail quickly.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetServiceTier(string value)
        {
            _serviceTier = value;
            return this;
        }

        public string ServiceTier => _serviceTier;

        /// <summary>
        /// Up to 4 sequences where the API will stop generating further tokens.
        /// The returned text will not contain the stop sequence.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetStop(string value)
        {
            _stopMessage = value;
            return this;
        }

        public string StopMessage => _stopMessage;

        /// <summary>
        /// The sampling temperature, between 0 and 1.
        /// Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// If set to 0, the model will use log probability to automatically increase the temperature until certain thresholds are hit.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[0 1]</param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetTemperature(float value)
        {
            _temperture = Mathf.Clamp01(value);
            return this;
        }

        public float Temperture => _temperture;


        /// <summary>
        /// Controls which (if any) tool is called by the model.
        /// none means the model will not call any tool and instead generates a message.
        /// auto means the model can pick between generating a message or calling one or more tools.
        /// required means the model must call one or more tools.
        /// Specifying a particular tool via {"type": "function = "function": {"name": "my_function"}} forces the model to call that tool.
        /// none is the default when no tools are present. auto is the default if tools are present.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetToolChoice(string value)
        {
            _toolChoice = value;
            return this;
        }

        public string ToolChoice => _toolChoice;

        /// <summary>
        /// This is not yet supported by any of our models.
        /// An integer between 0 and 20 specifying the number of most likely tokens to return at each token position,
        /// each with an associated log probability.
        /// logprobs must be set to true if this parameter is used.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[0 20] default=0</param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetTopLogprobs(int? value)
        {
            _topLogprob = value;
            return this;
        }

        public int? TopLogprob => _topLogprob;

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[-2.0 2.0] default=0</param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetTop_p(float? value)
        {
            _topP = value;
            return this;
        }

        public float? TopP => _topP;

        /// <summary>
        /// A unique identifier representing your end-user, which can help us monitor and detect abuse.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        public IAsyncChatRequest<(long statusCode, ChatCompletionResponse response)> SetUser(string value)
        {
            _user = value;
            return this;
        }

        public string User => _user;


        #endregion //) ===== IAsyncChatRequest =====

        [System.Serializable]
        public class RequestBody
        {
            // Required
            public string model;
            public RequestMessage[] messages;

            // Optional
            public float frequency_penalty;

            /* UnSupported */
            // public Object logit_bias;
            /* UnSupported */
            // public bool? logprobs;

            public int? max_completion_tokens = null;
            public int? n = null;
            public bool? parallel_tool_calls = null;
            public int? presence_penalty = null;
            public string reasoning_format = null;
            public int? seed = null;
            public string service_tier = null;
            public string stop = null;
            public float temperature = 0f;
            public string tool_choice = null;
            public int? top_logprobs = null;
            public float? top_p = null;
            public string user = null;
        }
        /// <summary>
        /// Data for request
        /// </summary>
        /// <remarks>In environments like IL2CPP on Android, information about derived types of abstract classes gets lost, so it is necessary to reassign them separately for the request.</remarks>
        [System.Serializable]
        public class RequestMessage
        {
            public string content;
            public string name;
            public string role;
            public string tool_call_id;

            public RequestMessage(BaseMessageContent msg)
            {
                this.content = msg.content;
                this.name = msg.name;
                this.role = msg.role;
                this.tool_call_id = msg.tool_call_id;
            }
        }

    }
}

