using AppointmentManagementService.Context;
using AppointmentManagementService.Entity;
using AppointmentManagementService.Interface;

namespace AppointmentManagementService.Service
{
    public class AppointmentService : IAppointment
    {
        private readonly AppointmentManagementServiceContext _context;

        public AppointmentService(AppointmentManagementServiceContext context)
        {
            _context = context;
        }



        public Task<AppointmentEntity> AddAppointment(AppointmentEntity appointment, int PatientID, int DoctorID)
        {
            throw new NotImplementedException();
        }
    }
}
