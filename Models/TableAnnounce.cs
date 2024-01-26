using System.Text.Json.Serialization;

namespace Dtr_Announce_App.Models;

public class AnnounceSubmission
{
    public IEnumerable<TableAnnounce>? Announces { get; set; }
    public string EncodedToken { get; set; } = string.Empty;
}

public class TableAnnounce
{
    [JsonPropertyName("number")]
    public int Id { get; set; }
    [JsonPropertyName("channelName")]
    public string Channel { get; set; } = string.Empty;
    [JsonPropertyName("announcementName")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("announceDate")]
    public string TextDate { get; set; } = string.Empty;
    [JsonPropertyName("tableBg")] 
    public string TableBg { get; set; } = string.Empty;
}