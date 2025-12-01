using Newtonsoft.Json;
using Graffity.Groq.Common;

namespace Graffity.Groq.Speech
{
    public class SpeechResponse
    {
        [JsonProperty("text")] public string Text { get; set; }

        [JsonProperty("x_groq")] public XGroq XGroq { get; set; }
    }
}