using UnityEngine;

namespace Graffity.Groq.Common
{
    [System.Serializable]
    public abstract class BaseMessageContent
    {
        public string content { get;protected set; }
        public string name { get;protected set; }
        public string role { get;protected set; }
        public string tool_call_id { get;protected set; }
    }
    /// <summary>
    /// SpeechRequest interface.
    /// </summary>
    /// <remarks> Setter should be method-chainable </remarks>
    /// <typeparam name="TResponse"></typeparam>
    public interface IAsyncChatRequest<TResponse> : IAsyncRequest<TResponse>
    {
        /// <summary>
        /// A list of messages comprising the conversation so far. 
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="message"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetPrompt(BaseMessageContent message);
        BaseMessageContent Prompt { get; }

        /// <summary>
        /// AI ModelType
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="modelType"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetModel(ChatAiModelType modelType);
        ChatAiModelType ModelType { get; }
        

        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize
        /// new tokens based on their existing frequency in the text so far,
        /// decreasing the model's likelihood to repeat the same line verbatim.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[-2.0 2.0] default=0</param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetFreqPenalty(float value);
        float FreqPenalty { get; }
        
        /// <summary>
        /// This is not yet supported by any of our models.
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetLogitBias(Object value);
        Object LogitBias { get; }


        
        /// <summary>
        /// This is not yet supported by any of our models.
        /// Whether to return log probabilities of the output tokens or not.
        /// If true, returns the log probabilities of each output token returned in the content of message.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetLogProbs(bool? value);
        bool? LogProbs { get; }

        /// <summary>
        /// The maximum number of tokens that can be generated in the chat completion.
        /// The total length of input tokens and generated tokens is limited by the model's context length.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="tokenCount"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetMaxCompletionTokens(int? tokenCount);
        int? MaxCompletionTokens { get; }
        
        /// <summary>
        /// How many chat completion choices to generate for each input message.
        /// Note that the current moment, only n=1 is supported. Other values will result in a 400 response.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="count"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetChoiceCount(int? count);
        int? N { get; }
        
        /// <summary>
        /// Whether to enable parallel function calling during tool use.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetParallelToolCalls(bool? value);
        bool? ParallelToolCalls { get; }
        
        /// <summary>
        /// Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on whether they appear in the text so far,
        /// increasing the model's likelihood to talk about new topics.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[-2.0 2.0] default=0</param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetPresencePenalty(int? value);
        int? PresencePenalty { get; }
        
        /// <summary>
        /// Specifies how to output reasoning tokens
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetReasoningFormat(string value);
        string ReasoningFormat { get; }
        
        /// <summary>
        /// If specified, our system will make a best effort to sample deterministically,
        /// such that repeated requests with the same seed and parameters should return the same result.
        /// Determinism is not guaranteed, and you should refer to the system_fingerprint response parameter
        /// to monitor changes in the backend.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[-2.0 2.0] default=0</param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetSeed(int? value);
        int? Seed { get; }
        
        /// <summary>
        /// The service tier to use for the request. Defaults to on_demand.
        /// auto will automatically select the highest tier available within the rate limits of your organization.
        /// flex uses the flex tier, which will succeed or fail quickly.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetServiceTier(string value);
        string ServiceTier { get; }
        
        /// <summary>
        /// Up to 4 sequences where the API will stop generating further tokens.
        /// The returned text will not contain the stop sequence.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetStop(string value);
        string StopMessage { get; }
        
        /// <summary>
        /// The sampling temperature, between 0 and 1.
        /// Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// If set to 0, the model will use log probability to automatically increase the temperature until certain thresholds are hit.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[0 1]</param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetTemperature(float value);
        float Temperture { get; }

        /// <summary>
        /// Controls which (if any) tool is called by the model.
        /// none means the model will not call any tool and instead generates a message.
        /// auto means the model can pick between generating a message or calling one or more tools.
        /// required means the model must call one or more tools.
        /// Specifying a particular tool via {"type": "function", "function": {"name": "my_function"}} forces the model to call that tool.
        /// none is the default when no tools are present. auto is the default if tools are present.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetToolChoice(string value);
        string ToolChoice { get; }
        
        /// <summary>
        /// This is not yet supported by any of our models.
        /// An integer between 0 and 20 specifying the number of most likely tokens to return at each token position,
        /// each with an associated log probability.
        /// logprobs must be set to true if this parameter is used.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[0 20] default=0</param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetTopLogprobs(int? value);
        int? TopLogprob { get; }
        
        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[-2.0 2.0] default=0</param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetTop_p (float? value);
        float? TopP { get; }
        
        /// <summary>
        /// A unique identifier representing your end-user, which can help us monitor and detect abuse.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value"></param>
        /// <returns>this</returns>
        IAsyncChatRequest<TResponse> SetUser(string value);
        string User { get; }
    }
}