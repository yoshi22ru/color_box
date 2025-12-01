using System;

namespace Graffity.Groq.Common
{
    /// <summary>
    /// https://console.groq.com/docs/speech-text
    /// Supported Models
    /// </summary>
    public enum SpeechAiModelType
    {
        /// <summary> Default Value. (for error)</summary>
        Undefined,

        /// <summary>
        /// Provides state-of-the-art performance with high accuracy for multilingual transcription and translation tasks.
        /// Supported Languages: Multilingual
        /// </summary>
        Whisper_large_v3,

        /// <summary>
        /// A fine-tuned version of a pruned Whisper Large V3 designed for fast, multilingual transcription tasks.
        /// Supported Languages: Multilingual
        /// </summary>
        Whisper_large_v3_turbo,

        /// <summary>
        /// A distilled, or compressed, version of OpenAI's Whisper model, designed to provide faster, lower cost English speech recognition while maintaining comparable accuracy.
        /// Supported Languages: English-only
        /// </summary>
        Distil_whisper_large_v3_en,

    }
    
    /// <summary>
    /// https://console.groq.com/docs/speech-text
    /// Supported Models
    /// </summary>
    public enum TTSAiModelType
    {
        /// <summary> Default Value. (for error)</summary>
        Undefined,

        /// <summary>
        /// Provides state-of-the-art performance with high accuracy for multilingual transcription and translation tasks.
        /// Supported Languages: Multilingual
        /// </summary>
        playai_tts,

        /// <summary>
        /// A fine-tuned version of a pruned Whisper Large V3 designed for fast, multilingual transcription tasks.
        /// Supported Languages: Multilingual
        /// </summary>
        playai_tts_arabic,
    }

    /// <summary>
    /// https://console.groq.com/docs/text-to-speech#available-english-voices
    /// EnglishVoiceType
    /// </summary>
    public enum TTSEnglishVoiceType
    {
        /// <summary> Default Value. (for error)</summary>
        Undefined,
        Arista_PlayAI,
        Atlas_PlayAI,
        Basil_PlayAI,
        Briggs_PlayAI,
        Calum_PlayAI,
        Celeste_PlayAI,
        Cheyenne_PlayAI,
        Chip_PlayAI,
        Cillian_PlayAI,
        Deedee_PlayAI,
        Fritz_PlayAI,
        Gail_PlayAI,
        Indigo_PlayAI,
        Mamaw_PlayAI,
        Mason_PlayAI,
        Mikail_PlayAI,
        Mitch_PlayAI,
        Quinn_PlayAI,
        Thunder_PlayAI
    }
    
    /// <summary>
    /// https://console.groq.com/docs/text-to-speech#available-arabic-voices
    /// Arabic VoiceType
    /// </summary>
    public enum TTSArabicVoiceType
    {
        /// <summary> Default Value. (for error)</summary>
        Undefined,
        Ahmad_PlayAI,
        Amira_PlayAI,
        Khalid_PlayAI,
        Nasser_PlayAI
    }
    public enum ChatAiModelType
    {
        Undefined,
        LLAMA_3_3_70b_versatile,
        
    }

    public static class ModelTypeExtensions
    {
        public static string ToModelID(this SpeechAiModelType self)
        {
            return self switch
            {
                SpeechAiModelType.Distil_whisper_large_v3_en => "distil-whisper-large-v3-en",
                SpeechAiModelType.Whisper_large_v3 => "whisper-large-v3",
                SpeechAiModelType.Whisper_large_v3_turbo => "whisper-large-v3-turbo",
                _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
            };
        }

        public static string ToWWWField(this TTSAiModelType self)
        {
            return self.ToString().Replace("_", "-");
        }

        public static string ToWWWField(this TTSEnglishVoiceType self)
        {
            return self.ToString().Replace("_", "-");
        }
        public static string ToWWWField(this TTSArabicVoiceType self)
        {
            return self.ToString().Replace("_", "-");
        }
    }
    
    public static class ChatAiModelTypeExtensions
    {
        public static string ToModelID(this ChatAiModelType self)
        {
            return self switch
            {
                ChatAiModelType.LLAMA_3_3_70b_versatile => "llama-3.3-70b-versatile",
                _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
            };
        }
    }
    
}