using Newtonsoft.Json;
using UnityEngine;

namespace Graffity.Groq.Models
{
    [System.Serializable]
    public class ModelListResponse
    {
        [JsonProperty("object")] public string Label { get; set; }
        [JsonProperty("data")] public ModelResponse[] Data { get; set; }
        
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(Label);
            foreach (var data in Data)
            {
                sb.AppendLine(data.ToString());
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
    
    [System.Serializable]
    public class ModelResponse
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("object")] public string ObjectName { get; set; }
        [JsonProperty("created")] public long CreatedAt { get; set; }
        [JsonProperty("owned_by")] public string OwnerName { get; set; }
        [JsonProperty("active")] public bool IsActive { get; set; }
        [JsonProperty("context_window")] public int ContextWindowSize { get; set; }
        [JsonProperty("public_apps")] public Object Apps { get; set; }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("ID:\t").AppendLine(Id);
            sb.Append("object:\t").AppendLine(ObjectName);
            sb.Append("owned_by:\t").AppendLine(OwnerName);
            sb.Append("active:\t").AppendLine(IsActive.ToString());
            sb.Append("context_window:\t").AppendLine(ContextWindowSize.ToString());

            return sb.ToString();
        }
    }
}