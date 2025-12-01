using Newtonsoft.Json;
using Graffity.Groq.Common;

namespace Graffity.Groq.Text
{
    [System.Serializable]
    public class ChatCompletionResponse
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("object")] public string Object { get; set; }

        [JsonProperty("created")] public long Created { get; set; }

        [JsonProperty("model")] public string Model { get; set; }

        [JsonProperty("choices")] public Choice[] Choices { get; set; }

        [JsonProperty("usage")] public Usage Usage { get; set; }

        [JsonProperty("system_fingerprint")] public string SystemFingerprint { get; set; }

        [JsonProperty("x_groq")] public XGroq XGroq { get; set; }
    }

    [System.Serializable]
    public class Choice
    {
        [JsonProperty("index")] public int Index { get; set; }

        [JsonProperty("message")] public Message Message { get; set; }

        [JsonProperty("logprobs")] public object Logprobs { get; set; }

        [JsonProperty("finish_reason")] public string FinishReason { get; set; }
    }

    [System.Serializable]
    public class Message
    {
        [JsonProperty("role")] public string Role { get; set; }

        [JsonProperty("content")] public string Content { get; set; }
    }

    [System.Serializable]
    public class Usage
    {
        [JsonProperty("queue_time")] public double QueueTime { get; set; }

        [JsonProperty("prompt_tokens")] public int PromptTokens { get; set; }

        [JsonProperty("prompt_time")] public double PromptTime { get; set; }

        [JsonProperty("completion_tokens")] public int CompletionTokens { get; set; }

        [JsonProperty("completion_time")] public double CompletionTime { get; set; }

        [JsonProperty("total_tokens")] public int TotalTokens { get; set; }

        [JsonProperty("total_time")] public double TotalTime { get; set; }
    }


}