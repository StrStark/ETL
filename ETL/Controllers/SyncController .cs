using ETL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ETL.Controllers
{
    [ApiController]
    [Route("api/sync/[action]")]
    public class SyncController : ControllerBase
    {
        private readonly SyncOrchestrator _orchestrator;

        public SyncController(SyncOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost("{entity}")]
        public async Task<IActionResult> DeleteSnapshot(string entity)
        {
            await _orchestrator.ClearSnapShot(entity);
            return Ok();
        }

        [HttpPost("{entity}")]
        public async Task<IActionResult> Sync(string entity)
        {
            var jobId = await _orchestrator.StartAsync(entity);
            return Accepted(new { jobId, entity });
        }

        [HttpPost("all")]
        public async Task<IActionResult> SyncAll()
        {
            var jobId = await _orchestrator.StartAllAsync();
            return Accepted(new { jobId });
        }
    }

}
