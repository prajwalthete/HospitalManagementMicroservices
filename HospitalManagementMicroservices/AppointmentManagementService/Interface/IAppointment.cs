using AppointmentManagementService.Dto;
using AppointmentManagementService.Entity;

namespace AppointmentManagementService.Interface
{
    public interface IAppointment
    {
        Task<AppointmentResponseModel> AddAppointment(AppointmentEntity appointment, int PatientID, int DoctorID);
        public Task<bool> DeleteAppointment(int appointmentId, int UserID);
        public Task<IEnumerable<AppointmentEntity>> GetAppointmentsByDoctor(int doctorId);
        public Task<IEnumerable<AppointmentEntity>> GetAppointmentsByPatient(int patientId);
        public Task<bool> UpdateAppointmentDate(int appointmentId, DateTime newDate);




    }
}
