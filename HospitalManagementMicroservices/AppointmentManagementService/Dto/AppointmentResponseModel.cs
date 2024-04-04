namespace AppointmentManagementService.Dto
{
    public class AppointmentResponseModel
    {

        public int AppointmentID { get; set; }
        public DateTime AppointmentDate { get; set; }

        public int DoctorID { get; set; }

        public int PatientID { get; set; }
    }
}
