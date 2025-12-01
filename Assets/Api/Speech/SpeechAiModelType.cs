namespace Graffity.Groq.Speech
{
    public enum SpeechAiModelType
    {
        Whisper_large_v3
    }

    public static class SpeechAiModelTypeExtensions
    {
        public static string ToModelID(this SpeechAiModelType type)
        {
            return type switch
            {
                SpeechAiModelType.Whisper_large_v3 => "whisper-large-v3",
                _ => throw new System.NotSupportedException()
            };
        }
    }
}
