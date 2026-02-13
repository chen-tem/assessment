using Assessment.Interfaces;
using Assessment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        private readonly ILogger<AssessmentController> _logger;
        private readonly IAssessmentService _assessmentService;

        public AssessmentController(ILogger<AssessmentController> logger, IAssessmentService assessmentService)
        {
            _logger = logger;
            _assessmentService = assessmentService;
        }

        /// <summary>
        /// Get a single Information record by UserId.
        /// If not found in DB, fetch from 3rd party API, save to DB, then return (local caching).
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>Information record</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInformationById(int id)
        {
            try
            {
                var info = await _assessmentService.GetByIdAsync(id);
                if (info == null)
                    return NotFound($"No record found for UserId {id} from DB or 3rd party API");
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving information for UserId {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Update an Information record by UserId
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="model">Updated Information</param>
        /// <returns>Result of update</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInformation(int id, [FromBody] InformationDto model)
        {
            if (model == null)
                return BadRequest("Invalid data");
            try
            {
                var updated = await _assessmentService.UpdateAsync(id, model);
                if (!updated)
                    return NotFound($"No record found for UserId {id}");
                return Ok("Record updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating information for UserId {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

    }

}
