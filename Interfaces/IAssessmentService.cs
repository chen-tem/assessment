using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Assessment.Interfaces
{
    public interface IAssessmentService
    {
        Task<IActionResult> GetInformationById(int id);
        Task<IActionResult> UpdateInformation(int id, [FromBody] Information model);
    }
}
