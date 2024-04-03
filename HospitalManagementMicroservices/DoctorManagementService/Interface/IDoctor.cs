using DoctorManagementService.Entity;

namespace DoctorManagementService.Interface
{
    public interface IDoctor
    {
        Task<DoctorEntity> AddDoctor(int DoctorID, DoctorEntity doctorEntity);
        Task<bool> DeleteDoctor(int DoctorID);
        Task<IEnumerable<DoctorEntity>> GetDoctorsBySpecialization(string specialization);
        Task<bool> UpdateSchedule(int doctorId, string newSchedule);



    }
}
