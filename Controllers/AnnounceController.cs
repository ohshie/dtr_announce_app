using Dtr_Announce_App.Business;
using Dtr_Announce_App.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dtr_Announce_App.Controllers;

[ApiController]
[Route("[controller]")]
public class AnnounceController(ILogger<AnnounceController> logger, 
    VkVideoUrlFetcher vkVideoUrlFetcher,
    AnnounceCreator announceCreator) : ControllerBase
{
    [HttpPost("CheckVkToken", Name = "CheckVkToken")]
    [EnableCors("CorsPolicy")]
    public async Task<IActionResult> CheckVkToken()
    {
        using var reader = new StreamReader(Request.Body);
        var vkTokenLink = await reader.ReadToEndAsync();
        vkTokenLink = Uri.UnescapeDataString(vkTokenLink);     
        
        var success = await vkVideoUrlFetcher.Execute(vkTokenLink, null);
        return StatusCode(success is not null ? 200 : 406);
    }

    [HttpPost("SubmitAnnounces", Name = "SubmitAnnounces")]
    [EnableCors("CorsPolicy")]
    public async Task<IActionResult> SubmitAnnounces([FromBody]AnnounceSubmission submission)
    {
        if (submission.Announces is null || !submission.Announces.Any())
        {
            return StatusCode(400);
        }

        if (string.IsNullOrWhiteSpace(submission.EncodedToken))
        {
            return StatusCode(400);
        }

        var vkTokenUrl = Uri.UnescapeDataString(submission.EncodedToken);
        var announces = submission.Announces;
        
        var success = await announceCreator.Execute(announces, vkTokenUrl);

        return StatusCode(success ? 200 : 406);
    }
}