using System.Text.Json.Serialization;

namespace Abyx.AI.EvaluationTests.Models;

public class EvaluationItem(string query, string groundTruth)
{
    [JsonPropertyName("query")]
    public string Query { get; set; } = query;
    
    [JsonPropertyName("ground_truth")]
    public string GroundTruth { get; set; } = groundTruth;
}