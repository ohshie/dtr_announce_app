using System.Text.Json.Serialization;

namespace Dtr_Announce_App.Models;

public class VKVideoInfo
{
    [JsonPropertyName("title")] 
    public string VideoName { get; set; } = string.Empty;
    [JsonPropertyName("player")] 
    public string VideoUrl { get; set; } = string.Empty;
    [JsonPropertyName("upload_date")] 
    public string UploadDate { get; set; } = string.Empty;
}