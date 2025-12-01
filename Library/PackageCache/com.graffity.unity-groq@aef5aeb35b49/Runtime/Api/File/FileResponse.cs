using Newtonsoft.Json;

namespace Graffity.Groq.File
{
    [System.Serializable]
    public class FileResponse
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("object")] public string ObjectType { get; set; }
        [JsonProperty("bytes")] public long ByteCount { get; set; }
        [JsonProperty("created_at")] public long CreatedAt { get; set; }
        [JsonProperty("filename")] public string FileName { get; set; }
        [JsonProperty("purpose")] public string Purpose { get; set; }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("ID:\t").AppendLine(Id);
            sb.Append("object:\t").AppendLine(ObjectType);
            sb.Append("bytes:\t").AppendLine(ByteCount.ToString());
            sb.Append("created_at:\t").AppendLine(CreatedAt.ToString());
            sb.Append("filename:\t").AppendLine(FileName);
            sb.Append("purpose:\t").AppendLine(Purpose);

            return sb.ToString();
        }
    }

    [System.Serializable]
    public class FileListResponse
    {
        [JsonProperty("object")] public string Label { get; set; }
        [JsonProperty("data")] public FileResponse[] Data { get; set; }

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
    public class FileDeleteResponse
    {
        [JsonProperty("id")] public string FileId { get; set; }
        [JsonProperty("object")] public string ObjectType { get; set; }
        [JsonProperty("deleted")] public bool IsDeleted { get; set; }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("ID:\t").AppendLine(FileId);
            sb.Append("object:\t").AppendLine(ObjectType);
            sb.Append("deleted:\t").AppendLine(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}