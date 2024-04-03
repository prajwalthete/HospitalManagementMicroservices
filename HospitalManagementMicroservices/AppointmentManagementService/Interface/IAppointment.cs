using AppointmentManagementService.Entity;

namespace AppointmentManagementService.Interface
{
    public interface IAppointment
    {
        Task<AppointmentEntity> AddAppointment(AppointmentEntity appointment, int PatientID, int DoctorID);

    }
}
