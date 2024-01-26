using System.Text.Json.Serialization;

namespace Dtr_Announce_App.Models;

public class Announce
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("date_pick_from")]
    public string StartDate { get; set; } = string.Empty;
    [JsonPropertyName("date_pick_to")]
    public string EndDate { get; set; } = string.Empty;
    [JsonPropertyName("remote_embed")]
    public string Url { get; set; } = string.Empty;
    [JsonPropertyName("schedule_date")]
    public string ScheduleDate { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string ChannelGroup { get; set; } = string.Empty;
}