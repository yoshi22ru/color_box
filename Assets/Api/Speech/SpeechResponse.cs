using Newtonsoft.Json;

namespace Graffity.Groq.Speech
{
    [System.Serializable]
    public class SpeechResponse
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
