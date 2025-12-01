using Newtonsoft.Json;

namespace Graffity.Groq.Common
{
    [System.Serializable]
    public class XGroq
    {
        [JsonProperty("id")] public string Id { get; set; }
    }
}