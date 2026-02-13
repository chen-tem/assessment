using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Assessment.Interfaces
{
    public interface IAssessmentService
    {
         Task<Information?> GetByIdAsync(int id);
         Task<bool> UpdateAsync(int id, Information updated);
    }
}

