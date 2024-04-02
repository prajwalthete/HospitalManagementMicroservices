using PatientManagementService.Dto;
using PatientManagementService.Entity;

namespace PatientManagementService.Interface
{
    public interface IPatient
    {
        public Task<PatientResponseModel> CreatePatientDetails(PatientEntity patientEntity, int UserID);
        public Task<PatientResponseModel> GetPatientById(int userId);


    }
}
