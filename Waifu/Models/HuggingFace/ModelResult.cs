using Newtonsoft.Json;

namespace Waifu.Models.HuggingFace;

public class ModelResult
{
    [JsonProperty("_id")] public string? MongoDbId { get; set; }

    [JsonProperty("id")] public string? Id { get; set; }

    [JsonProperty("author")] public string? Author { get; set; }

    [JsonProperty("lastModified")] public DateTime? LastModified { get; set; }

    [JsonProperty("likes")] public int? Likes { get; set; }

    [JsonProperty("private")] public bool? Private { get; set; }

    [JsonProperty("sha")] public string? Sha { get; set; }

    [JsonProperty("config")] public Config? Config { get; set; }

    [JsonProperty("downloads")] public int? Downloads { get; set; }

    [JsonProperty("tags")] public List<string> Tags { get; set; } = new();

    [JsonProperty("library_name")] public string? LibraryName { get; set; }

    [JsonProperty("createdAt")] public DateTime? CreatedAt { get; set; }

    [JsonProperty("modelId")] public string? ModelId { get; set; }

    [JsonProperty("siblings")] public List<Sibling> Siblings { get; set; } = new();
}

public class Config
{
    [JsonProperty("model_type")] public string? ModelType { get; set; }
}

public class Sibling
{
    [JsonProperty("rfilename")] public string? Rfilename { get; set; }
}