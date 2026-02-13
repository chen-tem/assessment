using Assessment.Modals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Assessment.Interfaces
{
    public interface IAssessmentService
    {
        Task<InformationDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, InformationDto updated);
    }
}
