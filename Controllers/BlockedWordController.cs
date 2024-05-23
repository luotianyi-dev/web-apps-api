using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using TianyiNetwork.Web.AppsApi.Models.Entities;
using TianyiNetwork.Web.AppsApi.Models.Transfer;
using TianyiNetwork.Web.AppsApi.Services;

namespace TianyiNetwork.Web.AppsApi.Controllers
{
    [ApiController]
    [Route("apps/api/blocked-word")]
    public class BlockedWordController(
        #pragma warning disable CS9113
        [SuppressMessage("ReSharper", "InconsistentNaming")] ILogger<BlockedWordController> logger,
        [SuppressMessage("ReSharper", "InconsistentNaming")] BlockedWordService BlockedWord,
        [SuppressMessage("ReSharper", "InconsistentNaming")] Entity db) : ControllerBase
        #pragma warning restore CS9113
    {
        [HttpGet]
        public ActionResult<IEnumerable<BlockedWordGroup>> GetBlockedWordGroups()
        {
            return Ok(BlockedWord.Groups);
        }
    }
}
