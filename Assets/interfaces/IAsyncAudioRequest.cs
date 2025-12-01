using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Graffity.Groq.Common;

namespace Graffity.Groq.Common
{
    /// <summary>
    /// SpeechRequest interface.
    /// </summary>
    /// <remarks> Setter should be method-chainable </remarks>
    /// <typeparam name="TResponse"></typeparam>
    public interface IAsyncSpeechRequest<TResponse> : IAsyncRequest<TResponse>
    {
        /// <summary>
        /// Audio File Path
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="filePath"></param>
        /// <returns>this</returns>
        IAsyncSpeechRequest<TResponse> SetFilePath(string filePath);
        string AudioFilePath { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="modelType"></param>
        /// <returns>this</returns>
        IAsyncSpeechRequest<TResponse> SetModel(SpeechAiModelType modelType);
        SpeechAiModelType ModelType { get; }
        
        /// <summary>
        /// The language of the input audio.
        /// Supplying the input language in ISO-639-1 format will improve accuracy and latency.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="language"></param>
        /// <returns>this</returns>
        IAsyncSpeechRequest<TResponse> SetLanguage(string language);
        string Language { get; }

        /// <summary>
        /// Prompt text
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="prompt"></param>
        /// <returns>this</returns>
        IAsyncSpeechRequest<TResponse> SetPrompt(string prompt);
        string PromptText { get; }

        /// <summary>
        /// PThe sampling temperature, between 0 and 1.
        /// Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// If set to 0, the model will use log probability to automatically increase the temperature until certain thresholds are hit.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="value">[0 1]</param>
        /// <returns>this</returns>
        IAsyncSpeechRequest<TResponse> SetTemperature(float value);
        float Temperture { get; }

    }
    
    /// <summary>
    /// TTS api interface.
    /// </summary>
    /// <remarks> Setter should be method-chainable </remarks>
    /// <typeparam name="TResponse"></typeparam>
    public interface IAsyncTextToSpeechRequest<TResponse> : IAsyncRequest<TResponse>
    {
        /// <summary>
        /// AI Model Type
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="modelType"></param>
        /// <returns>this</returns>
        IAsyncTextToSpeechRequest<TResponse> SetModel(TTSAiModelType modelType);
        TTSAiModelType ModelType { get; }
        
        /// <summary>
        /// The language of the input audio.
        /// Supplying the input language in ISO-639-1 format will improve accuracy and latency.
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="input"></param>
        /// <returns>this</returns>
        IAsyncTextToSpeechRequest<TResponse> SetText(string input);
        string Text { get; }

        /// <summary>
        /// VoiceType
        /// </summary>
        /// <remarks>Required</remarks>
        /// <param name="voice">Use TTSEnglishVoiceType/TTSArbicVoiceType toString()</param>
        /// <returns>this</returns>
        IAsyncTextToSpeechRequest<TResponse> SetVoice(string voice);
        string Voice { get; }

        /// <summary>
        /// Audio Format 
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="format">default: wav</param>
        /// <returns>this</returns>
        IAsyncTextToSpeechRequest<TResponse> SetAudioFormat(string format);
        string AudioFormat { get; }

        /// <summary>
        /// The speed of the generated audio. 1.0 is the only supported value.
        /// </summary>
        /// <remarks>Optional</remarks>
        /// <param name="speed">default: 1.0</param>
        /// <returns>this</returns>
        IAsyncTextToSpeechRequest<TResponse> SetSpeed(float speed);
        float Speed { get; }


        
    }
}