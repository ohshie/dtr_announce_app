using Dtr_Announce_App.Models;
using Microsoft.Extensions.Logging;

namespace Dtr_Announce_App.Business;

public class AnnounceCreator(ILogger<AnnounceCreator> logger,
    VkVideoUrlFetcher vkVideoUrlFetcher,
    AnnouncePoster announcePoster,
    DateConverter dateConverter)
{
    public async Task<bool> Execute(IEnumerable<TableAnnounce> tableAnnounces, string vkUrlToken)
    {
        var announces = MapAnnounces(tableAnnounces).ToList();
        dateConverter.Execute(announces);
        await MapVideoUrlsToAnnounces(announces, vkUrlToken);
        
        if (announces.Any(a => string.IsNullOrEmpty(a.Url)))
        {
            return false;
        }

        return await PostAnnounces(announces);
    }
    
    private IEnumerable<Announce> MapAnnounces(IEnumerable<TableAnnounce> tableAnnounces)
    {   
        List<Announce> announces = new();
        
        foreach (var announce in tableAnnounces)
        {
            var properAnnounce = new Announce()
            {
                Title = announce.Name.TrimEnd(),
                Channel = announce.Channel,
                StartDate = DateTime.Today.ToString("dd.MM.yyyy"),
                EndDate = announce.TextDate,
                ScheduleDate = announce.TextDate,
            };

            ChannelGroupSorter(properAnnounce);
            
            announces.Add(properAnnounce);
        }

        return announces;
    }
    
    private async Task MapVideoUrlsToAnnounces(List<Announce> announces, string vkUrlToken)
    {
        foreach (var group in announces.GroupBy(a => a.ChannelGroup))
        {
            var namesAndUrls = await vkVideoUrlFetcher.Execute(vkUrlToken, group.Key, group.Count()) as List<VKVideoInfo>;;
            
            if (namesAndUrls is null || namesAndUrls.Count == 0)
            {
                logger.LogError("Failed to get any video URL's from vk for {GroupName}", group.Key);
                return;
            }

            foreach (var announce in group)
            {
                var nameAndUrl = namesAndUrls.Find(a => a.VideoName == announce.Title);
                if (nameAndUrl is null)
                {
                    logger.LogError("Tried to find {AnnounceTitle} in {GroupName} but none was found", announce.Title, group.Key);
                    return;
                }
                
                announce.Url = nameAndUrl.VideoUrl.Split("&hash")[0];
            }
        }
    }

    private async Task<bool> PostAnnounces(IEnumerable<Announce> announces)
    {
        var groupedAnnounces = announces.GroupBy(a => a.Channel);

        var errorInGroups = string.Empty;
        
        foreach (var announce in groupedAnnounces)
        {
            logger.LogInformation("Attempting to create {AmountOfAnnounces} announces on {SiteName}", 
                announce.Count(),announce.Key);

            var success = await announcePoster.Create(announce);

            if (!success)
            {
                errorInGroups += announce.Key;
            }
        }

        return (string.IsNullOrEmpty(errorInGroups));
    }
    
    private void ChannelGroupSorter(Announce announce)
    {
        announce.ChannelGroup = announce.Channel switch
        {
            "Фан" => "Фан",
            "МЗК" => "МЗК",
            "НСТ" => "НСТ",
            _ => "Кино"
        };
    }
}